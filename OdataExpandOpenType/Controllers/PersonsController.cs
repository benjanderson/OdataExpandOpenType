namespace OdataExpandOpenType.Controllers
{
  using System.Threading.Tasks;
  using System.Web.Http;
  using System.Web.Http.OData;
  using System.Web.OData.Routing;

  public class PersonsController : ODataController
  {
    public PersonsController()
    {
      this.dbContext = new PersonContext();
    }

    private readonly PersonContext dbContext;

    [HttpPost]
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
