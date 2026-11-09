namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.Business;
using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Xunit;
using Fixture = Tmobis.Business.Process.Documents.Tests.EvaluationSheetReviewTests.Fixture;

public sealed class EvaluationSheetReadAccessTests
{
    static EvaluationSheetReadAccessTests()
    {
        ICSSoft.STORMNET.Windows.Forms.ExternalLangDef.LanguageDef =
            new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(null!);
    }

    [Theory]
    [InlineData("sheet", "WorkObject", "Awp")]
    [InlineData("requirement", "EvaluationSheet.WorkObject", "EvaluationSheet.Awp")]
    [InlineData("sign", "Act.WorkObject", "Act.Awp")]
    public async Task CommissionReadAccessIsScopedToBothOrganizationAndPeriod(
        string entity, string objectPath, string awpPath)
    {
        var fixture = new Fixture(AdmissionCommitteeRoleNames.Member);
        fixture.AddCommissionMember();
        var otherOrganization = new Organization();
        var otherPeriod = new Awp();
        fixture.Members.Add(new AdmissionCommitteeEmployee
        {
            Employee = new Employee { Id = fixture.Access.EmployeeId },
            AdmissionCommittee = new AdmissionCommittee
            {
                Organization = otherOrganization, Awp = otherPeriod, Status = AdmissionCommitteeStatus.Approved
            }
        });
        var additional = new EvaluationSheetCommissionReadLimitProvider(fixture.Permissions, null!);
        var provider = new EvaluationSheetReadRestrictionProvider(new TestReadLimit(), [additional]);
        var entityType = entity == "sheet" ? typeof(EvaluationSheet)
            : entity == "requirement" ? typeof(EvaluationSheetRequirement) : typeof(EvaluationSheetSign);
        var restriction = await provider.GetRestriction(entityType);
        Assert.Contains(awpPath, restriction.RequiredProperties);
        Assert.Contains($"{objectPath}.Organization.Hierarchy", restriction.RequiredProperties);
        Assert.True(Matches(restriction.LimitFunction!, fixture.Sheet.WorkObject.Organization.Id, fixture.Sheet.Awp.Id));
        Assert.True(Matches(restriction.LimitFunction!, Guid.NewGuid(), fixture.Sheet.Awp.Id,
            fixture.Sheet.WorkObject.Organization.Id));
        Assert.True(Matches(restriction.LimitFunction!, otherOrganization.Id, otherPeriod.Id));
        Assert.False(Matches(restriction.LimitFunction!, fixture.Sheet.WorkObject.Organization.Id, otherPeriod.Id));
        Assert.False(Matches(restriction.LimitFunction!, otherOrganization.Id, fixture.Sheet.Awp.Id));
        Assert.False(Matches(restriction.LimitFunction!, Guid.NewGuid(), fixture.Sheet.Awp.Id));
        Assert.True((await fixture.Permissions.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanReview);
        var view = new View { DefineClassType = entityType };
        foreach (var property in restriction.RequiredProperties) view.AddProperty(property);
        var lcs = LoadingCustomizationStruct.GetSimpleStruct(entityType, view);
        lcs.LimitFunction = restriction.LimitFunction;
        var sql = new PostgresDataService(new ICSSoft.STORMNET.Security.EmptySecurityManager(),
            Stub<ICSSoft.STORMNET.Business.Audit.IAuditService>(),
            Stub<ICSSoft.STORMNET.Business.Interfaces.IBusinessServerProvider>()).GenerateSQLSelect(lcs, false);
        Assert.Contains(fixture.Sheet.Awp.Id.ToString(), sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(fixture.Sheet.WorkObject.Organization.Id.ToString(), sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CommissionReadAccessDoesNotGrantOrdinaryRoleAccessOrIgnoreApproval()
    {
        var fixture = new Fixture(AdmissionCommitteeRoleNames.Member);
        fixture.AddCommissionMember();
        fixture.Members[0].AdmissionCommittee.Status = AdmissionCommitteeStatus.Editing;
        var additional = new EvaluationSheetCommissionReadLimitProvider(fixture.Permissions, null!);
        Assert.Null(await additional.GetReadLimit("WorkObject", "Awp"));
        fixture.Members[0].AdmissionCommittee.Status = AdmissionCommitteeStatus.Approved;
        fixture.Members[0].Employee = new Employee();
        Assert.Null(await additional.GetReadLimit("WorkObject", "Awp"));
        fixture.Members[0].Employee.Id = fixture.Access.EmployeeId;
        var objectProvider = new WorkObjectODataReadRestrictionProvider(new TestReadLimit(), [additional]);
        var restriction = await objectProvider.GetRestriction(typeof(WorkObject));
        Assert.True(Matches(restriction.LimitFunction!, fixture.Sheet.WorkObject.Organization.Id, Guid.NewGuid()));
        Assert.False(Matches(restriction.LimitFunction!, Guid.NewGuid(), fixture.Sheet.Awp.Id));
        Assert.False((await fixture.Permissions.GetPermissions(fixture.Sheet.WorkObject.Id, Guid.NewGuid())).CanReview);
        Assert.False((await fixture.Permissions.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanFormAct);
        var nonMember = new Fixture(WorkObjectRoleNames.Consumer);
        nonMember.AddCommissionMember();
        Assert.Null(await new EvaluationSheetCommissionReadLimitProvider(nonMember.Permissions, null!)
            .GetReadLimit("WorkObject", "Awp"));
    }

    [Fact]
    public async Task AdditionalReadAccessPreservesOrdinaryAccessAndUnrestrictedAdministrator()
    {
        var fixture = new Fixture(AdmissionCommitteeRoleNames.Member);
        fixture.AddCommissionMember();
        var additional = new EvaluationSheetCommissionReadLimitProvider(fixture.Permissions, null!);
        var ordinaryOrganization = new Organization();
        var language = new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(null!);
        var ordinary = FunctionBuilder.BuildEquals(new VariableDef(language.GuidType, "WorkObject.Organization"), ordinaryOrganization.Id);
        var restriction = await new TestReadLimit(ordinary).GetReadRestriction([additional], "WorkObject", "Awp");
        Assert.True(Matches(restriction.LimitFunction!, ordinaryOrganization.Id, Guid.NewGuid()));
        Assert.True(Matches(restriction.LimitFunction!, fixture.Sheet.WorkObject.Organization.Id, fixture.Sheet.Awp.Id));
        restriction = await new TestReadLimit(unrestricted: true).GetReadRestriction([additional], "WorkObject", "Awp");
        Assert.Null(restriction.LimitFunction);
        Assert.Empty(restriction.RequiredProperties);
    }

    private static bool Matches(Function function, Guid organizationId, Guid awpId, Guid? hierarchyId = null)
    {
        object? Resolve(object parameter) => parameter is VariableDef variable
            ? variable.StringedView.EndsWith("Organization.Hierarchy") ? hierarchyId
                : variable.StringedView.EndsWith("Organization") ? organizationId
                : variable.StringedView.EndsWith("Awp") ? awpId
                    : Guid.Parse("00000000-0000-0000-0000-000000000001")
            : parameter;
        bool Equal(object? left, object? right) => left != null && right != null
            && left.ToString()!.Trim('{', '}').Equals(right.ToString()!.Trim('{', '}'), StringComparison.OrdinalIgnoreCase);
        var parameters = function.Parameters.Cast<object>().ToList();
        return function.FunctionDef.StringedView switch
        {
            "OR" => parameters.Cast<Function>().Any(x => Matches(x, organizationId, awpId, hierarchyId)),
            "AND" => parameters.Cast<Function>().All(x => Matches(x, organizationId, awpId, hierarchyId)),
            "=" => Equal(Resolve(parameters[0]), Resolve(parameters[1])),
            "IN" => parameters.Skip(1).Any(value => Equal(Resolve(parameters[0]), Resolve(value))),
            _ => throw new NotSupportedException(function.FunctionDef.StringedView)
        };
    }

    private static T Stub<T>() where T : class
    {
        var proxy = DispatchProxy.Create<T, EvaluationSheetReviewTests.RepositoryProxy>();
        ((EvaluationSheetReviewTests.RepositoryProxy)(object)proxy).Handler = (method, _) =>
            method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null;
        return proxy;
    }

    private sealed class TestReadLimit(Function? limit = null, bool unrestricted = false) : IWorkObjectReadLimitProvider
    {
        public Task<Function?> GetReadLimit(string path)
        {
            var language = new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(null!);
            return Task.FromResult(unrestricted ? null : limit
                ?? FunctionBuilder.BuildEquals(new VariableDef(language.GuidType, path), Guid.Empty));
        }
        public Task<Function?> GetReadLimit(View view, string connectMasterProperty, string ownerConnectProperty)
            => throw new NotSupportedException();
    }

}
