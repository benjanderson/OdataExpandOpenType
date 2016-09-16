namespace OdataExpandOpenType.Controllers
{
  using System.Data.Entity;
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
      this.dbContext = new PersonContext();
    }

    private readonly PersonContext dbContext;

	  [HttpGet]
		[ResponseType(typeof(IQueryable<Person>))]
		[ODataRoute("Persons")]
	  [Route("api/Persons")]
	  public IHttpActionResult Get()
	  {
		  return this.Ok(this.dbContext.Persons.Include(p => p.Attributes));
	  }

	  [HttpPost]
		[ResponseType(typeof(Person))]
    [ODataRoute("Persons")]
    [Route("api/Persons")]
    public async Task<IHttpActionResult> Post([FromBody] Person person)
    {
      this.dbContext.Persons.Add(person);
      await this.dbContext.SaveChangesAsync();
      return this.Ok(person);
    }
  } 
}
