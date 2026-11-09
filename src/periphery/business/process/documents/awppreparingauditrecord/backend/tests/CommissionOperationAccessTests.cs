namespace Tmobis.Business.Process.Documents.Tests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Process.Documents.Controllers;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Business.Process.Documents.Providers;
using Xunit;

public sealed class CommissionOperationAccessTests
{
    [Fact]
    public async Task CannotOperateForAnotherPeriod()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);

        await AssertOperationIsRejectedBeforeUpdates(fixture, awpId: Guid.NewGuid());
    }

    [Fact]
    public async Task CannotOperateOutsideTerritory()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);
        var outsideTerritory = fixture.Add(new WorkObject
        {
            Organization = new Common.Contacts.Organization(), Actuality = true,
        });

        await AssertOperationIsRejectedBeforeUpdates(fixture, workObjectId: outsideTerritory.Id);
    }

    [Fact]
    public async Task CannotOperateWithoutStoredCommissionMembership()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture, addMembership: false);

        await AssertOperationIsRejectedBeforeUpdates(fixture);
    }

    [Fact]
    public async Task CannotOperateWithoutCommissionRole()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);
        fixture.Roles.Names = [WorkObjectRoleNames.Administration];

        await AssertOperationIsRejectedBeforeUpdates(fixture);
    }

    [Fact]
    public async Task OrdinaryCommissionMemberCannotPerformChairOperation()
    {
        var fixture = new DocumentBoundaryFixture();
        var committee = AddCommittee(fixture);
        committee.Leader = null;

        await AssertOperationIsRejectedBeforeUpdates(fixture);
    }

    [Fact]
    public async Task AnotherEmployeeCannotPerformCommissionOperation()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);

        await AssertOperationIsRejectedBeforeUpdates(fixture, employeeId: Guid.NewGuid());
    }

    [Fact]
    public async Task UnapprovedCommitteeCannotPerformObjectOperation()
    {
        var fixture = new DocumentBoundaryFixture();
        var committee = AddCommittee(fixture);
        committee.Status = AdmissionCommitteeStatus.Editing;

        await AssertOperationIsRejectedBeforeUpdates(fixture);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ChairAndDeputyGetApplicableObjectsWithoutOtherRolePrerequisites(bool deputy)
    {
        var fixture = new DocumentBoundaryFixture();
        var committee = AddCommittee(fixture);
        if (deputy)
        {
            committee.DeputyLeader = committee.Leader;
            committee.Leader = null;
        }
        fixture.Object.ObjectFunction = new ObjectFunction();
        var program = fixture.Add(new AwpAssessingProgram
        {
            Organization = fixture.Municipality, Awp = fixture.Awp, Committee = committee,
            ActStatus = new ActStatus { Id = IActStatus.Keys.Approved },
        });
        program.Schedules.Add(new AwpAssessingProgramEvaluationSchedule
        {
            Program = program, ObjectFunction = fixture.Object.ObjectFunction,
        });

        var result = Assert.IsType<OkObjectResult>(await Controller(fixture, new()).GetAvailableObjects(
            fixture.Awp.Id, fixture.Users.Employee.Id));
        Assert.Contains(fixture.Object.Id, Assert.IsType<List<Guid>>(result.Value));
        var provider = new Preparation();
        Assert.IsType<OkResult>(await Controller(fixture, provider).UpdateRecordsForObjects(
            fixture.Awp.Id, fixture.Users.Employee.Id, fixture.Object.Id));
        Assert.Equal(1, provider.Updates);
        Assert.Equal(1, fixture.Commits);
        var records = await fixture.Repository.Find<AwpPreparingAuditRecord>(_ => true);
        Assert.Equal(fixture.Object.Id, Assert.Single(records).WorkObject.Id);
    }

    [Fact]
    public async Task WrongPeriodCannotUseAvailableObjectsOrBulkEndpoint()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);
        var provider = new Preparation();
        var controller = Controller(fixture, provider);
        Assert.IsType<ForbidResult>(await controller.GetAvailableObjects(Guid.NewGuid(), fixture.Users.Employee.Id));
        Assert.IsType<ForbidResult>(await controller.UpdateRecords(Guid.NewGuid(), fixture.Users.Employee.Id));
        Assert.Equal(0, provider.Updates);
    }

    [Fact]
    public async Task ProgramFromDifferentCommitteeCannotEnableObjectOperation()
    {
        var fixture = new DocumentBoundaryFixture();
        AddCommittee(fixture);
        var program = fixture.Add(new AwpAssessingProgram
        {
            Organization = fixture.Municipality, Awp = fixture.Awp, Committee = new AdmissionCommittee(),
            ActStatus = new ActStatus { Id = IActStatus.Keys.Approved },
        });
        program.Schedules.Add(new AwpAssessingProgramEvaluationSchedule { ObjectFunction = new ObjectFunction() });
        var provider = new Preparation();
        var result = await Controller(fixture, provider).UpdateRecordsForObjects(
            fixture.Awp.Id, fixture.Users.Employee.Id, fixture.Object.Id);
        Assert.IsNotType<OkResult>(result);
        Assert.Equal(0, provider.Updates);
    }

    private static AdmissionCommittee AddCommittee(DocumentBoundaryFixture fixture, bool addMembership = true)
    {
        fixture.Roles.Names = [AdmissionCommitteeRoleNames.Member];
        var committee = fixture.Add(new AdmissionCommittee
        {
            Organization = fixture.Municipality, Awp = fixture.Awp, Status = AdmissionCommitteeStatus.Approved,
        });
        var member = new AdmissionCommitteeEmployee
        {
            AdmissionCommittee = committee, Employee = fixture.Users.Employee,
        };
        if (addMembership) fixture.Add(member);
        committee.Leader = member;
        return committee;
    }

    private static AwpPreparingAuditRecordController Controller(DocumentBoundaryFixture fixture, Preparation provider)
        => new(fixture.Repository, provider,
            new AwpPreparingAuditRecordCommissionAccess(
                fixture.Repository,
                fixture.Access,
                fixture.Users,
                new AdmissionCommitteeMembershipProvider(
                    fixture.Repository, fixture.Roles, fixture.Users)),
            NullLogger<AwpPreparingAuditRecordController>.Instance);

    private static async Task AssertOperationIsRejectedBeforeUpdates(
        DocumentBoundaryFixture fixture,
        Guid? awpId = null,
        Guid? employeeId = null,
        Guid? workObjectId = null)
    {
        var provider = new Preparation();
        var controller = Controller(fixture, provider);

        Assert.IsType<ForbidResult>(await controller.UpdateRecordsForObjects(
            awpId ?? fixture.Awp.Id,
            employeeId ?? fixture.Users.Employee.Id,
            workObjectId ?? fixture.Object.Id));
        Assert.Equal(0, provider.Updates);
        Assert.Equal(0, fixture.Commits);
    }

    private sealed class Preparation : IAwpPreparingAuditRecordProvider
    {
        public int Updates { get; private set; }
        public Task UpdatePartnerRecords(Guid awpId, Guid organizationId)
        {
            Updates++;
            return Task.CompletedTask;
        }
        public Task<IReadOnlyList<WorkObjectPreparationInfo>> GetObjectPreparationInfo(
            Guid? awpId, IReadOnlyCollection<Guid> workObjectIds) => throw new NotSupportedException();
    }
}
