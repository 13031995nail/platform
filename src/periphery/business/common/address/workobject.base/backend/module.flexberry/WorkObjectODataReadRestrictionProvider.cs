namespace Tmobis.Business.Common.Address;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

/// <summary>
/// Ограничения чтения объектов и их связей с ТСО.
/// </summary>
public sealed class WorkObjectODataReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider,
    IEnumerable<IWorkObjectAdditionalReadLimitProvider> additionalProviders)
    : IODataReadRestrictionProvider
{
    /// <inheritdoc />
    public bool Supports(Type entityType)
        => entityType == typeof(WorkObject)
            || entityType == typeof(WorkObjectTsoOrganization);

    /// <inheritdoc />
    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
    {
        var propertyPath = entityType == typeof(WorkObject)
            ? ODataReadRestriction.PrimaryKeyProperty
            : nameof(WorkObjectTsoOrganization.WorkObject);
        return await readLimitProvider.GetReadRestriction(additionalProviders, propertyPath);
    }
}
