namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;

/// <summary>
/// Строит ограничения чтения объектов работ для инфраструктурных запросов.
/// </summary>
public interface IWorkObjectReadLimitProvider
{
    Task<Function?> GetReadLimit(string workObjectPropertyPath);

    Task<Function?> GetReadLimit(
        View detailView,
        string connectMasterProperty,
        string ownerConnectProperty);
}
