namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Core.Models;
using Xunit;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class AssessingProgramAccessTests
{
    [Fact]
    public async Task GeneratedAssessingProgramDetailsAndSignersDoNotRequireStoredRows()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var program = new AwpAssessingProgram { Organization = fixture.Organization };

        await fixture.ProgramAccess.EnsureCanChange(new AwpAssessingProgramObject
        {
            Program = program, WorkObject = fixture.Object,
        });
        await fixture.ProgramAccess.EnsureCanChange(new AwpAssessingProgramSign { Act = program });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AssessingProgramReassignmentChecksBothParentOrganizationScopes(bool oldParentForeign)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var foreign = fixture.AddObject(new Organization());
        var oldOrganization = oldParentForeign ? foreign.Organization : fixture.Organization;
        var newOrganization = oldParentForeign ? fixture.Organization : foreign.Organization;
        var detail = new AwpAssessingProgramObject
        {
            Program = new AwpAssessingProgram { Organization = newOrganization }, WorkObject = fixture.Object,
        };
        fixture.Add(new AwpAssessingProgramObject
        {
            Id = detail.Id, Program = new AwpAssessingProgram { Organization = oldOrganization },
            WorkObject = fixture.Object,
        });
        DocumentActionScopeTestSupport.MarkAltered(detail);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.ProgramAccess.EnsureCanChange(detail));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ProgramSignChecksOldAndNewScopesWithBatchReads(bool allowAll)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var oldObject = fixture.AddObject(new Organization());
        fixture.Allow(fixture.Object, allowAll ? oldObject : fixture.Object);
        var oldProgram = fixture.Add(new AwpAssessingProgram { Organization = oldObject.Organization });
        var newProgram = fixture.Add(new AwpAssessingProgram { Organization = fixture.Organization });
        var sign = new AwpAssessingProgramSign { Act = newProgram };
        DocumentActionScopeTestSupport.MarkAltered(sign);
        fixture.Add(new AwpAssessingProgramSign { Id = sign.Id, Act = oldProgram });

        if (allowAll)
        {
            await fixture.ProgramAccess.EnsureCanChange(sign);
        }
        else
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                fixture.ProgramAccess.EnsureCanChange(sign));
        }

        Assert.Equal(1, fixture.Reads.Count(type => type == typeof(AwpAssessingProgram)));
        Assert.Equal(1, fixture.Reads.Count(type => type == typeof(AwpAssessingProgramObject)));
        Assert.Equal(1, fixture.Reads.Count(type => type == typeof(WorkObject)));
    }
}
