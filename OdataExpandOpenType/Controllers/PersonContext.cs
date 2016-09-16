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
			this.SaveDynamicProperties<Person, PersonAttribute>();
			this.SaveDynamicProperties<Widget, WidgetAttribute>();
			return base.SaveChangesAsync();
		}

		private void SaveDynamicProperties<T, TU>()
		 where T : class, IOpenType
			where TU : Attribute, new()
		{
			Func<DbEntityEntry<T>, bool> expression = e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Unchanged;
			var modifiedModels = this.ChangeTracker.Entries<T>().Where(expression).ToList();
			foreach (var openTypeInstance in modifiedModels)
			{
				var attributeCollection = openTypeInstance.Collection<TU>("Attributes");

				if (openTypeInstance.State != EntityState.Added)
				{
					attributeCollection.Load();
				}

				ICollection<TU> currentAttributes = attributeCollection.CurrentValue;

				//this.DeleteDynamicProperties(currentAttributes, openTypeInstance);

				foreach (var dynamicPropertyKeyValue in openTypeInstance.Entity.Properties)
				{
					var existing = currentAttributes.FirstOrDefault(current => current.Name == dynamicPropertyKeyValue.Key);
					if (existing == null)
					{
						var newObjectAttributeValue = new TU
							                              {
								                              Name = dynamicPropertyKeyValue.Key,
								                              Value = dynamicPropertyKeyValue.Value.ToString()
							                              };
						currentAttributes.Add(newObjectAttributeValue);
					}
					else
					{
						existing.Value = dynamicPropertyKeyValue.Value.ToString();
					}
				}
			}
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


	public class Person : IOpenType
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


  public class Widget : IOpenType
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

	public interface IOpenType
	{
		[Key]
		int Id { get; set; }

		IDictionary<string, object> Properties { get; }
	}
}
