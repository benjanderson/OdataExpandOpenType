namespace OdataExpandOpenType.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.OData.Builder;

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
        [AutoExpand]
        public virtual ICollection<Widget> Widgets { get; set; }

        [NotMapped]
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

        [NotMapped]
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
}
