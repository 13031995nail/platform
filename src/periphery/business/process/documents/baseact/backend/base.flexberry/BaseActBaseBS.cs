namespace Tmobis.Business.Process.Documents.Base.Flexberry;
using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

public class BaseActBaseBS(IMessageDispatcher messageDispatcher)
: MessageBasedEntityBS<IBaseActBase>(messageDispatcher)
{
  public virtual DataObject[] OnUpdateBaseActBase(BaseActBase UpdatedObject)
  => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

  public static Guid? GetStoredStatusId(
    BaseActBase document,
    BaseActBase? storedDocument)
  {
    var dataCopy = storedDocument?.GetDataCopy() as BaseActBase;
    return dataCopy?.ActStatus?.Id
      ?? (ReferenceEquals(document, storedDocument) ? null : storedDocument?.ActStatus?.Id);
  }

  public static void EnsureValidStatusTransition(
    BaseActBase document,
    Guid? storedStatusId,
    bool allSigned,
    bool currentEmployeeIsBaseSigner,
    bool allowApproval = false)
  {
    var statusId = document.ActStatus?.Id;
    var allowed = statusId == IActStatus.Keys.Signing
      ? (storedStatusId == IActStatus.Keys.Editing
          || storedStatusId == IActStatus.Keys.Signed)
        && currentEmployeeIsBaseSigner
      : statusId == IActStatus.Keys.Signed
        ? (storedStatusId == IActStatus.Keys.Editing
            || storedStatusId == IActStatus.Keys.Signing) && allSigned
        : statusId == IActStatus.Keys.Approved
          && allowApproval
          && storedStatusId == IActStatus.Keys.Signed
          && allSigned
          && currentEmployeeIsBaseSigner;
    if (!allowed)
    {
      throw new System.UnauthorizedAccessException(
        "Не выполнены условия подписания или утверждения документа.");
    }
  }
}
