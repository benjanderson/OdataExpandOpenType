﻿using System;
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

        public IDbSet<Sub1Person> Sub1Persons { get; set; }

        public IDbSet<Sub2Person> Sub2Persons { get; set; }

        public IDbSet<Widget> Widgets { get; set; }

        public IDbSet<PersonAttribute> PersonAttributes { get; set; }

        public IDbSet<WidgetAttribute> WidgetAttributes { get; set; }        
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
        public Person()
        {
            this.Attributes = new HashSet<PersonAttribute>();
            this.Widgets = new HashSet<Widget>();
            this.Type = string.IsNullOrEmpty(this.Type) ? nameof(Person) : this.Type;
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Contained]
        public virtual ICollection<Widget> Widgets { get; set; }

        [Contained]
        public virtual ICollection<PersonAttribute> Attributes { get; }
    }

    public class Sub1Person : Person
    {
        public Sub1Person()
        {
            this.Type = nameof(Sub1Person);
        }

        public string Foo { get; set; }
    }

    public class Sub2Person : Person
    {
        public Sub2Person()
        {
            this.Type = nameof(Sub2Person);
        }

        public string Bar { get; set; }
    }

    public class Widget 
    {
        public Widget()
        {
            this.Attributes = new HashSet<WidgetAttribute>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }

        [Contained]
        public virtual ICollection<WidgetAttribute> Attributes { get; }        
    }    
}
