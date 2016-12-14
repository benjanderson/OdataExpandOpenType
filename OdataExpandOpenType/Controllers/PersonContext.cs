namespace OdataExpandOpenType.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.OData.Builder;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class PersonContext
    {
        public PersonContext(string connectionString)
        {
            this.Persons = new MongoDBRepository<Person>(connectionString);
            this.Widgets = new MongoDBRepository<Widget>(connectionString);
        }

        public MongoDBRepository<Person> Persons { get; set; }

        public MongoDBRepository<Widget> Widgets { get; set; }
    }

    public class Attribute
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class PersonAttribute : Attribute
    {
        [ForeignKey("Person")]
        public int PersonId { get; set; }
    }

    public class WidgetAttribute : Attribute
    {
        public virtual Widget Widget { get; set; }

        [ForeignKey("Widget")]
        public int WidgetId { get; set; }
    }


    public class Person : BaseEntity
    {
        private IDictionary<string, object> properties;

        public Person()
        {
            this.Attributes = new List<PersonAttribute>();
            this.Widgets = new HashSet<Widget>();
        }
        
        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Contained]
        public virtual ICollection<Widget> Widgets { get; set; }

        [Contained]
        public ICollection<PersonAttribute> Attributes { get; }

        public IDictionary<string, object> Properties
        {
            get
            {
                if (this.properties == null)
                {
                    if (this.Attributes == null)
                    {
                        return new Dictionary<string, object>();
                    }

                    this.properties = this.Attributes.ToDictionary(dynamicProperty => dynamicProperty.Name, dynamicProperty => (object)dynamicProperty.Value);
                }

                return this.properties;
            }

            set
            {
                this.properties = value;
            }
        }
    }


    public class Widget : BaseEntity
    {
        private IDictionary<string, object> properties;

        public Widget()
        {
            this.properties = new Dictionary<string, object>();
            this.Attributes = new Collection<WidgetAttribute>();
        }

        public string Name { get; set; }

        public double Value { get; set; }

        public ICollection<WidgetAttribute> Attributes { get; }

        public IDictionary<string, object> Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = this.Attributes.ToDictionary(dynamicProperty => dynamicProperty.Name, dynamicProperty => (object)dynamicProperty.Value);
                }

                return this.properties;
            }
        }
    }

    public class MongoDBRepository<T> where T : BaseEntity
    {
        public MongoDBRepository(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var databaseName = new MongoUrl(connectionString).DatabaseName;
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<T>(typeof(T).Name);
        }

        protected IMongoDatabase _database;
        public IMongoDatabase Database
        {
            get
            {
                return _database;
            }
        }

        protected IMongoCollection<T> _collection;
        public IMongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(string id)
        {
            return this._collection.Find(e => e.Id == id).FirstOrDefaultAsync().Result;
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual T Insert(T entity)
        {
            this._collection.InsertOne(entity);
            return entity;
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Insert(entity);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual T Update(T entity)
        {
            var update = this._collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = false }).Result;
            return entity;

        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Update(entity);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            this._collection.FindOneAndDeleteAsync(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                this._collection.FindOneAndDeleteAsync(e => e.Id == entity.Id);
            }
        }

        public virtual IMongoQueryable<T> Table
        {
            get { return this._collection.AsQueryable(); }
        }
    }

    [BsonIgnoreExtraElements]
    public abstract class BaseEntity : ParentEntity
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }

   public abstract class ParentEntity
    {
        public ParentEntity()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _id = ObjectId.GenerateNewId().ToString();
                else
                    _id = value;
            }
        }

        private string _id;

    }
}
