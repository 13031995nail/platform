namespace Tmobis.Business.Core.Flexberry.Consumers.Files;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;

public class OnFileDeletedConsumer(IRepository repository)
: IMessageConsumer<EntityDeleted<IFile>, IEntityUpdatedResult>
{
  public async Task<IEntityUpdatedResult> ConsumeWithResult(
    EntityDeleted<IFile> message)
  {
    ArgumentNullException.ThrowIfNull(message);
    var file = message.Entity;
    var oldFile = await GetOldFile(file.Id);
    var isProtected = IsProtected(oldFile);
    if (isProtected)
    {
      var dataObject = file as DataObject;
      dataObject?.SetStatus(ObjectStatus.UnAltered);
    }

    return new EntityUpdatedResult { Continue = !isProtected };
  }

  protected virtual async Task<IFile> GetOldFile(Guid id)
  => await repository.GetById<IFile>(id)
    ?? throw new InvalidOperationException($"Не найден файл {id} в репозитории");

  protected virtual bool IsProtected(IFile file)
  => file?.IsDeleteProtected ?? false;
}
