namespace OdataExpandOpenType.Controllers
{
    using System.Collections.Generic;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class MongoDBRepository<T> where T : BaseEntity
    {
        public MongoDBRepository(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var databaseName = new MongoUrl(connectionString).DatabaseName;
            this._database = client.GetDatabase(databaseName);
            this._collection = this._database.GetCollection<T>(typeof(T).Name);
        }

        protected IMongoDatabase _database;
        public IMongoDatabase Database
        {
            get
            {
                return this._database;
            }
        }

        protected IMongoCollection<T> _collection;
        public IMongoCollection<T> Collection
        {
            get
            {
                return this._collection;
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
                this.Insert(entity);
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
                this.Update(entity);
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
}