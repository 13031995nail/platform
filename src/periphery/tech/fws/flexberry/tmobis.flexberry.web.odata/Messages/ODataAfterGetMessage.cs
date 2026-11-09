namespace Tmobis.Frameworks.Flexberry.Web.OData.Messages
{
    using ICSSoft.STORMNET;
    using Tmobis.Core.Medium.Models;

    public record ODataAfterGetMessage : Message
    {
        public List<DataObject>? Objects { get; init; }
    }
}
