namespace Tmobis.Frameworks.Flexberry.Web.OData.Messages
{
    using ICSSoft.STORMNET.Business;
    using Tmobis.Core.Medium.Models;

    public record ODataBeforeGetMessage : Message
    {
        public LoadingCustomizationStruct? Lcs { get; init; }
    }
}
