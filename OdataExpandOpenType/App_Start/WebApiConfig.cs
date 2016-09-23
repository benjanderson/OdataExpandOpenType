namespace OdataExpandOpenType
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using System.Web.OData.Query;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;
    using Microsoft.Restier.Core.Model;
    using Microsoft.Restier.Core.Submit;
    using Microsoft.Restier.Providers.EntityFramework;
    using Microsoft.Restier.Publishers.OData.Batch;
    using Microsoft.Restier.Publishers.OData;
    using Microsoft.Restier.Publishers.OData.Model;

    using OdataExpandOpenType.Controllers;

    internal static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterApi(config, GlobalConfiguration.DefaultServer);
            var odataFormatters = ODataMediaTypeFormatters.Create(new CustomODataSerializerProvider(), new DefaultODataDeserializerProvider());
            config.Formatters.InsertRange(0, odataFormatters);
        }

        public static async void RegisterApi(
            HttpConfiguration config, HttpServer server)
        {
            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
            await config.MapRestierRoute<PersonApi>("PersonsApi", "api", new RestierBatchHandler(server));
        }
    }
}
