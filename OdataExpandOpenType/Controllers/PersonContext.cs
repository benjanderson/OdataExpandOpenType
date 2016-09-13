using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdataExpandOpenType.Controllers
{
  using System.Collections.ObjectModel;
  using System.Data.Entity;
  using System.Web.OData.Builder;

  public class PersonContext : DbContext
  {
    public PersonContext()
    {
      this.Database.CreateIfNotExists();
    }

    public IDbSet<Person> Persons { get; set; }

    public IDbSet<Widget> Widgets { get; set; }

    public IDbSet<Attribute> Attributes { get; set; }
  }

  public class Attribute
  {
    public int Id { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public object Value { get; set; }
  }

  public class Person
  {
    private IDictionary<string, object> properties;

    public Person()
    {
      this.properties = new Dictionary<string, object>();
      this.Attributes = new HashSet<Attribute>();
      this.Widgets = new HashSet<Widget>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime DateOfBirth { get; set; }

    [Contained]
    public virtual ICollection<Widget> Widgets { get; set; }

    public ICollection<Attribute> Attributes { get; }

    public IDictionary<string, object> Properties
    {
      get
      {
        if (this.properties == null)
        {
          this.properties = this.Attributes.ToDictionary(dynamicProperty => dynamicProperty.Name, dynamicProperty => dynamicProperty.Value);
        }

        return this.properties;
      }
    }
  }


  public class Widget
  {
    private IDictionary<string, object> properties;

    public Widget()
    {
      this.properties = new Dictionary<string, object>();
      this.Attributes = new Collection<Attribute>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public double Value { get; set; }

    public ICollection<Attribute> Attributes { get; }

    public IDictionary<string, object> Properties
    {
      get
      {
        if (this.properties == null)
        {
          this.properties = this.Attributes.ToDictionary(dynamicProperty => dynamicProperty.Name, dynamicProperty => dynamicProperty.Value);
        }

        return this.properties;
      }
    }
  }
}
