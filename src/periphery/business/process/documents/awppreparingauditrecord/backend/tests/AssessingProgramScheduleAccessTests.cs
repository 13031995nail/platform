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

public sealed class AssessingProgramScheduleAccessTests
{
    [Fact]
    public async Task TsoCannotChangeProgramSchedule()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        fixture.Allow(fixture.Object);
        var program = new AwpAssessingProgram { Organization = fixture.Organization };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.ProgramAccess.EnsureCanChange(new AwpAssessingProgramEvaluationSchedule
            {
                Program = program,
            }));
    }

    [Fact]
    public async Task AdministrationCanSaveScheduleOfNewProgram()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);

        await fixture.ProgramAccess.EnsureCanChange(new AwpAssessingProgramEvaluationSchedule
        {
            Program = new AwpAssessingProgram { Organization = fixture.Organization },
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ProgramScheduleChecksStoredParentWhenReassignedOrDeleted(bool deleted)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var oldProgram = fixture.Add(new AwpAssessingProgram
        {
            Organization = fixture.AddObject(new Organization()).Organization,
        });
        oldProgram.SetStatus(ObjectStatus.UnAltered);
        var schedule = new AwpAssessingProgramEvaluationSchedule
        {
            Program = new AwpAssessingProgram { Organization = fixture.Organization },
        };
        fixture.Add(new AwpAssessingProgramEvaluationSchedule { Id = schedule.Id, Program = oldProgram });
        DocumentActionScopeTestSupport.MarkAltered(schedule);
        schedule.SetStatus(deleted ? ObjectStatus.Deleted : ObjectStatus.Altered);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.ProgramAccess.EnsureCanChange(schedule));
    }

    [Fact]
    public async Task ProgramScheduleReadRestrictionUsesParentProgram()
    {
        var limits = new DocumentScheduleReadLimits();
        var provider = new AwpAssessingProgramReadRestrictionProvider(limits);

        Assert.True(provider.Supports(typeof(AwpAssessingProgramEvaluationSchedule)));
        await provider.GetRestriction(typeof(AwpAssessingProgramEvaluationSchedule));

        Assert.Equal(nameof(AwpAssessingProgramObject.Program), limits.MasterProperty);
        Assert.Equal(nameof(AwpAssessingProgramEvaluationSchedule.Program), limits.OwnerProperty);
    }
}
