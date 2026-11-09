namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Web.OData.Access;

/// <summary>
/// Строит OData-ограничение для документа, его объекта и подписанта.
/// </summary>
public static class WorkObjectODataRestrictionFactory
{
    private const string WorkObjectProperty = nameof(WorkObject);

    public static async Task<ODataReadRestriction> Create(
        IWorkObjectReadLimitProvider readLimitProvider,
        Type entityType,
        Type detailEntityType,
        Type documentEntityType,
        View detailView,
        string connectMasterProperty,
        string signDocumentProperty)
    {
        if (entityType == detailEntityType)
        {
            return ODataReadRestriction.ForProperty(
                await readLimitProvider.GetReadLimit(WorkObjectProperty),
                WorkObjectProperty);
        }

        var ownerProperty = entityType == documentEntityType
            ? ODataReadRestriction.PrimaryKeyProperty
            : signDocumentProperty;
        return ODataReadRestriction.ForProperty(
            await readLimitProvider.GetReadLimit(
                detailView,
                connectMasterProperty,
                ownerProperty),
            ownerProperty);
    }
}
