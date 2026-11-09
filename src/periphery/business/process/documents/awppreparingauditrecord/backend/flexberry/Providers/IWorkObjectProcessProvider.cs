namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Process.Documents.Models;

public interface IWorkObjectProcessProvider
{
    Task<WorkObjectProcess> GetProcess(Guid workObjectId, Guid? awpId);

    Task<WorkObjectPreparationInfo> GetPreparation(Guid workObjectId, Guid? awpId);
}
