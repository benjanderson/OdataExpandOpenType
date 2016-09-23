using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdataExpandOpenType.Controllers
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Web.OData.Builder;

    public class PersonContext : DbContext
    {
        public PersonContext()
        {
            this.Database.CreateIfNotExists();
            this.Configuration.LazyLoadingEnabled = false;
        }

        public IDbSet<Person> Persons { get; set; }

        public IDbSet<Widget> Widgets { get; set; }

        public IDbSet<PersonAttribute> PersonAttributes { get; set; }

        public IDbSet<WidgetAttribute> WidgetAttributes { get; set; }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }

    public class Attribute
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class PersonAttribute : Attribute
    {
        public virtual Person Person { get; set; }

        [ForeignKey("Person")]
        public int PersonId { get; set; }
    }

    public class WidgetAttribute : Attribute
    {
        public virtual Widget Widget { get; set; }

        [ForeignKey("Widget")]
        public int WidgetId { get; set; }
    }


    public class Person 
    {
        private IDictionary<string, object> properties;

        public Person()
        {
            this.Attributes = new List<PersonAttribute>();
            this.Widgets = new HashSet<Widget>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Contained]
        public virtual ICollection<Widget> Widgets { get; set; }

        [Contained]
        public ICollection<PersonAttribute> Attributes { get; }        
    }


    public class Widget 
    {
        private IDictionary<string, object> properties;

        public Widget()
        {
            this.properties = new Dictionary<string, object>();
            this.Attributes = new Collection<WidgetAttribute>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }

        public ICollection<WidgetAttribute> Attributes { get; }
    }    
}
