namespace OdataExpandOpenType.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.OData;
    using System.Web.OData.Routing;

    [EnableQuery]
    public class PersonsController : ODataController
    {
        public PersonsController()
        {
            this.dbContext = new PersonContext(ConfigurationManager.ConnectionStrings["PersonContext"].ConnectionString);
        }

        private readonly PersonContext dbContext;

        [HttpGet]
        [ResponseType(typeof(IQueryable<Person>))]
        [ODataRoute("Persons")]
        [Route("api/Persons")]
        public IHttpActionResult Get()
        {
            var people = this.dbContext.Persons.Table.ToList();
            return this.Ok(people.AsQueryable());
        }

        [HttpPost]
        [ResponseType(typeof(Person))]
        [ODataRoute("Persons")]
        [Route("api/Persons")]
        public IHttpActionResult Post([FromBody] Person person)
        {
            this.dbContext.Persons.Insert(person);
            return this.Ok(person);
        }

        [HttpPut]
        [ResponseType(typeof(Person))]
        [ODataRoute("Persons")]
        [Route("api/Persons")]
        public IHttpActionResult Put([FromBody] Person person)
        {
            this.dbContext.Persons.Update(person);
            return this.Ok(person);
        }


        [HttpPatch]
        [ResponseType(typeof(Person))]
        [ODataRoute("Persons")]
        [Route("api/Persons")]
        public IHttpActionResult Patch([FromBody] Delta<Person> person)
        {
            object personId;
            if (!person.TryGetPropertyValue("Id", out personId))
            {
                throw new InvalidOperationException("no id");
            }

            var databasePerson = this.dbContext.Persons.GetById((string)personId);
            person.Patch(databasePerson);
            this.dbContext.Persons.Update(databasePerson);
            return this.Ok(person);
        }
    }
}
