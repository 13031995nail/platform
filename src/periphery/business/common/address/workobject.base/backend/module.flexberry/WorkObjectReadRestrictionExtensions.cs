namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Web.OData.Access;

/// <summary>
/// Объединение основных и дополнительных полномочий чтения на стороне SQL.
/// </summary>
public static class WorkObjectReadRestrictionExtensions
{
    public static async Task<ODataReadRestriction> GetReadRestriction(
        this IWorkObjectReadLimitProvider provider,
        IEnumerable<IWorkObjectAdditionalReadLimitProvider> additionalProviders,
        string workObjectPropertyPath, string? awpPropertyPath = null)
    {
        var limit = await provider.GetReadLimit(workObjectPropertyPath);
        if (limit == null)
        {
            return ODataReadRestriction.ForProperty(null, workObjectPropertyPath);
        }
        var hasAdditionalAccess = false;
        foreach (var additionalProvider in additionalProviders)
        {
            var additional = await additionalProvider.GetReadLimit(workObjectPropertyPath, awpPropertyPath);
            if (additional != null)
            {
                limit = FunctionBuilder.BuildOr(limit, additional);
                hasAdditionalAccess = true;
            }
        }
        var properties = ODataReadRestriction.ForProperty(limit, workObjectPropertyPath)
            .RequiredProperties.ToList();
        if (hasAdditionalAccess)
        {
            var prefix = workObjectPropertyPath == ODataReadRestriction.PrimaryKeyProperty
                ? string.Empty : $"{workObjectPropertyPath}.";
            properties.Add($"{prefix}Organization");
            properties.Add($"{prefix}Organization.Hierarchy");
            if (awpPropertyPath != null) properties.Add(awpPropertyPath);
        }
        return new(limit, properties.Distinct().ToList());
    }
}
