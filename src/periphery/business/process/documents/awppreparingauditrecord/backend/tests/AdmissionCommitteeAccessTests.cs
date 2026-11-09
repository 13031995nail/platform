namespace Tmobis.Business.Process.Documents.Tests;

using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.TownLimit;
using Xunit;

public sealed class AdmissionCommitteeAccessTests
{
    [Fact]
    public async Task CommissionMemberCannotManageCommission()
    {
        var access = CreateAccess(["Член комиссии"], Guid.NewGuid(), true);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            access.EnsureCanChange(new AdmissionCommittee
            {
                Organization = new Organization(),
            }));
    }

    [Fact]
    public async Task BusinessAdministratorCannotManageForeignTerritory()
    {
        var access = CreateAccess(
            ["Контролирующий орган"],
            Guid.NewGuid(),
            organizationInTown: false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            access.EnsureCanChange(new AdmissionCommittee
            {
                Organization = new Organization(),
            }));
    }

    [Fact]
    public async Task TechnicalAdministratorBypassesTerritoryRestriction()
    {
        var access = CreateAccess(["admin"], null, false);

        await access.EnsureCanChange(new AdmissionCommittee());
    }

    private static AdmissionCommitteeAccess CreateAccess(
        string[] roles,
        Guid? townId,
        bool organizationInTown)
    {
        var repository = TestRepository.Create((method, _) => method.Name switch
        {
            "Any" => Task.FromResult(organizationInTown),
            _ => throw new NotSupportedException(method.Name),
        });
        return new AdmissionCommitteeAccess(
            new TestRoles(roles),
            new TestTown(townId),
            repository);
    }

    private sealed class TestRoles(string[] roles) : ICurrentRoleProvider
    {
        public ValueTask<List<string>> GetCurrentRoleNames()
            => ValueTask.FromResult(roles.ToList());
    }

    private sealed class TestTown(Guid? townId) : ICurrentTownProvider
    {
        public ValueTask<Guid?> GetCurrentTownId()
            => ValueTask.FromResult(townId);
    }
}
