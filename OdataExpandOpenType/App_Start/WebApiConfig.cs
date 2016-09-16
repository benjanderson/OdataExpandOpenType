namespace OdataExpandOpenType
{
  using System.Web.Http;
  using System.Web.OData.Builder;
  using System.Web.OData.Extensions;

  using OdataExpandOpenType.Controllers;

  internal static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      // OData routes
      ODataModelBuilder builder = new ODataConventionModelBuilder
      {
        Namespace = "OdataExpandOpenType",
        ContainerName = "OdataExpandOpenTypeContainer"
      };
      builder.EntitySet<Person>("Persons");

      config.MapODataServiceRoute("ODataRoute", "api", builder.GetEdmModel());

	    config.EnsureInitialized();
    }
  }
}
