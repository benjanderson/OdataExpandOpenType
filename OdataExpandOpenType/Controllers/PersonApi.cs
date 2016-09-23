using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdataExpandOpenType.Controllers
{
    using System.Net.Http;
    using System.Web.OData.Formatter.Serialization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Restier.Providers.EntityFramework;
    using Microsoft.Restier.Publishers.OData.Formatter;

    public class PersonApi : EntityFrameworkApi<PersonContext>
    {
        public PersonApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }

    public class CustomOdataSerializer : DefaultRestierSerializerProvider
    {
        public CustomOdataSerializer(IServiceProvider rootContainer)
            : base(rootContainer)
        {
        }

        public override ODataSerializer GetODataPayloadSerializer(Type type, HttpRequestMessage request)
        {
            return base.GetODataPayloadSerializer(type, request);
        }
    }
}
