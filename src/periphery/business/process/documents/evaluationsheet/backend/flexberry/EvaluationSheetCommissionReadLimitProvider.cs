namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.FunctionalLanguage;
using Tmobis.Business.Common.Address;
using Tmobis.Frameworks.Flexberry.Web.OData.Access;

/// <summary>
/// Ограничение чтения комиссией по утверждённому составу, организации объекта и ОЗП.
/// </summary>
public sealed class EvaluationSheetCommissionReadLimitProvider(
    EvaluationSheetPermissionProvider permissionProvider,
    IDataService dataService) : IWorkObjectAdditionalReadLimitProvider
{
    public async Task<Function?> GetReadLimit(string workObjectPropertyPath, string? awpPropertyPath = null)
    {
        var members = await permissionProvider.GetCommissionMemberships();
        var language = new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(dataService);
        var prefix = workObjectPropertyPath == ODataReadRestriction.PrimaryKeyProperty
            ? string.Empty : $"{workObjectPropertyPath}.";
        var restrictions = new List<Function>();
        foreach (var period in members.GroupBy(x => x.AdmissionCommittee.Awp.Id))
        {
            var organizations = period.Select(x => x.AdmissionCommittee.Organization.Id)
                .Distinct().Cast<object>().ToArray();
            foreach (var batch in organizations.Chunk(500))
            {
                var organizationLimit = FunctionBuilder.BuildOr(
                    FunctionBuilder.BuildIn(new VariableDef(language.GuidType, $"{prefix}Organization"), batch),
                    FunctionBuilder.BuildIn(new VariableDef(language.GuidType, $"{prefix}Organization.Hierarchy"), batch));
                restrictions.Add(awpPropertyPath == null ? organizationLimit : FunctionBuilder.BuildAnd(
                    organizationLimit,
                    FunctionBuilder.BuildEquals(new VariableDef(language.GuidType, awpPropertyPath), period.Key)));
            }
        }
        return restrictions.Count == 0 ? null : FunctionBuilder.BuildOr(restrictions);
    }
}
