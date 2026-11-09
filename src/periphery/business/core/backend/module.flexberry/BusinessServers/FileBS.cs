namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;
using File = Models.File;

[ICSSoft.STORMNET.AccessType(ICSSoft.STORMNET.AccessType.none)]
public class FileBS(IMessageDispatcher messageDispatcher)
: MessageBasedEntityBS<IFile>(messageDispatcher)
{
  public virtual DataObject[] OnUpdateFile(File UpdatedObject)
  => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];
}
