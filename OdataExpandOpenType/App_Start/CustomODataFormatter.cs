namespace OdataExpandOpenType.App_Start
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.OData;
    using System.Web.OData.Formatter.Serialization;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    using OdataExpandOpenType.Controllers;
    using System.Linq;

    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm.Library;

    public class CustomODataSerializerProvider : DefaultODataSerializerProvider
    {
        private readonly AnnotatingEntitySerializer annotatingEntitySerializer;

        public CustomODataSerializerProvider()
        {
            this.annotatingEntitySerializer = new AnnotatingEntitySerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsEntity())
            {
                return this.annotatingEntitySerializer;
            }

            return base.GetEdmTypeSerializer(edmType);
        }

        public override ODataSerializer GetODataPayloadSerializer(IEdmModel model, Type type, HttpRequestMessage request)
        {
            return base.GetODataPayloadSerializer(model, type, request);
        }
    }

    public class AnnotatingEntitySerializer : ODataEntityTypeSerializer
    {
        public AnnotatingEntitySerializer(ODataSerializerProvider serializerProvider)
            : base(serializerProvider)
        {
        }

        public override ODataEntry CreateEntry(SelectExpandNode selectExpandNode, EntityInstanceContext entityInstanceContext)
        {
            ODataEntry entry = base.CreateEntry(selectExpandNode, entityInstanceContext);
            Person document = entityInstanceContext.EntityInstance as Person;
            if (entry != null && document != null)
            {
                var navigationProperty =
                    selectExpandNode.SelectedNavigationProperties.FirstOrDefault(nav => nav.Name == "Attributes");
                if (navigationProperty != null && selectExpandNode.ExpandedNavigationProperties.All(n => n.Key.Name != "Attributes"))
                {
                    // Uncomment the following lines to display attributes
                    var expandClause = new SelectExpandClause(null, true);
                    selectExpandNode.ExpandedNavigationProperties.Add(navigationProperty, expandClause);
                    selectExpandNode.SelectedNavigationProperties.Remove(navigationProperty);
                }
            }

            return entry;
        }

        public override SelectExpandNode CreateSelectExpandNode(EntityInstanceContext entityInstanceContext)
        {
            return base.CreateSelectExpandNode(entityInstanceContext);
        }
    }
}