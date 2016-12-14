namespace OdataExpandOpenType.Controllers
{
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
            return this.Ok(this.dbContext.Persons.Table);
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
    }
}
