namespace Tmobis.Frameworks.Flexberry.Web.OData.Messages;

using ICSSoft.STORMNET;
using Tmobis.Core.Medium.Models;

public record ODataBeforeUpdateMessage : Message
{
    public DataObject? Object { get; init; }
}
