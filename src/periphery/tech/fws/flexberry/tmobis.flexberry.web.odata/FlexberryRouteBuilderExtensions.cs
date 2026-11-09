using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using NewPlatform.Flexberry.ORM.ODataService;
using NewPlatform.Flexberry.ORM.ODataService.Extensions;
using NewPlatform.Flexberry.ORM.ODataService.Model;
using Tmobis.Frameworks.Flexberry.Web.OData;
using Tmobis.Frameworks.Flexberry.Web.OData.Batch;

namespace Tmobis.Frameworks.Flexberry.Web.OData;

public static class FlexberryRouteBuilderExtensions
{
    public static ManagementToken MapFlexberryDataObjectRoute(this IRouteBuilder builder, IServiceProvider serviceProvider, IDataObjectEdmModelBuilder modelBuilder, string routeName = "odata", string routePrefix = "odata", int messageQuotasMaxPartsPerBatch = 1000, int messageQuotasMaxOperationsPerChangeset = 1000, long messageQuotasMaxReceivedMessageSize = long.MaxValue)
    {
        var batchHandler = serviceProvider.GetService<FlexberryODataBatchHandler>();
        var messageQuotas = new ODataMessageQuotas
        {
            MaxPartsPerBatch = messageQuotasMaxPartsPerBatch,
            MaxOperationsPerChangeset = messageQuotasMaxOperationsPerChangeset,
            MaxReceivedMessageSize = messageQuotasMaxReceivedMessageSize,
        };
        return builder.MapDataObjectRoute(modelBuilder, batchHandler, delegate (IContainerBuilder cb)
        {
            cb.AddServicePrototype(new ODataMessageReaderSettings
            {
                MessageQuotas = messageQuotas,
            });
        }, routeName, routePrefix);
    }
}