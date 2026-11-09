namespace Tmobis.Business.Core.Flexberry.Models;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Core.Models.Towns;

[BusinessServer(
  "Tmobis.Business.Core.Flexberry.BusinessServers.LocalDictionaryBS, Tmobis.Business.Core.Flexberry",
  DataServiceObjectEvents.OnAllEvents)]
public interface IFlexberryWTown : IWTown
{
}
