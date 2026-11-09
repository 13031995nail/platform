namespace Tmobis.Business.Process.Documents.Tests;

using System.Reflection;
using System.Linq.Expressions;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using IUser = Tmobis.Business.Core.Models.Users.IUser;
using IEmployee = Tmobis.Business.Core.Models.Employees.IEmployee;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using Xunit;

public sealed class EvaluationSheetReviewTests
{

    [Fact]
    public async Task RoleAccessUsesLoadedOrganizationAndOnlyChecksTsoLinkInDatabase()
    {
        var town = new Town();
        var organization = new Organization { Town = town };
        var workObject = new WorkObject { Organization = organization };
        var users = new TestUsers(new User { Town = town }, new Employee());
        var places = new List<PlaceOfEmployment> { new() { Organization = organization } };
        var links = new List<WorkObjectTsoOrganization> { new() { WorkObject = workObject, Organization = organization } };
        var reads = 0;
        var repository = DispatchProxy.Create<IRepository, RepositoryProxy>();
        ((RepositoryProxy)repository).Handler = (method, args) =>
        {
            reads++;
            return method.Name switch
            {
                "Find" when method.GetGenericArguments()[0] == typeof(PlaceOfEmployment) => Task.FromResult(places),
                "Any" when method.GetGenericArguments()[0] == typeof(WorkObjectTsoOrganization)
                    => Task.FromResult(links.Any(((Expression<Func<WorkObjectTsoOrganization, bool>>)args![0]!).Compile())),
                _ => throw new NotSupportedException(method.Name)
            };
        };
        var provider = new WorkObjectAccessProvider(users,
            new TestRoles([WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso, WorkObjectRoleNames.Administration]), repository, null!);
        Assert.Equal(new WorkObjectRoleAccess(true, true, true), await provider.GetRoleAccess(workObject));
        Assert.Equal(2, reads);
        var foreignObject = new WorkObject { Organization = new Organization { Town = new Town() } };
        Assert.Equal(new WorkObjectRoleAccess(false, false, false), await provider.GetRoleAccess(foreignObject));
        Assert.Equal(3, reads);
    }

    [Fact]
    public async Task UnknownOperationIsRejectedBeforeLoadingSheetOrCheckingAccess()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "unknown"));
        Assert.Equal(0, fixture.SheetByIdReads);
        Assert.Equal(0, fixture.Access.RoleAccessCalls);
        Assert.False(fixture.Committed);
    }

    [Fact]
    public async Task ObjectPermissionsDoNotLoadObjectSeparatelyForEachRole()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso, WorkObjectRoleNames.Administration);
        var permissions = await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id);
        Assert.True(permissions.CanSubmit);
        Assert.True(permissions.CanReview);
        Assert.Equal(1, fixture.WorkObjectReads);
        Assert.Equal(1, fixture.Access.RoleAccessCalls);
        Assert.Equal(0, fixture.Access.RoleIdCalls);
    }

    [Fact]
    public async Task BeginningReviewPreservesSubmissionDateAndCannotBeRepeated()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        var requested = DateTime.UtcNow.AddDays(-2);
        fixture.Sheet.ReviewRequestedAt = requested;
        await fixture.Service.Review(fixture.Sheet.Id, "review");
        var started = fixture.Sheet.ReviewStartedAt;
        Assert.Equal(requested, fixture.Sheet.ReviewRequestedAt);
        Assert.NotNull(started);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "review"));
        Assert.Equal(started, fixture.Sheet.ReviewStartedAt);
    }

    [Fact]
    public async Task AcceptanceRequiresSubmissionAndBeginningReview()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
        Assert.False(fixture.Committed);
        fixture.Sheet.ReviewRequestedAt = DateTime.UtcNow;
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
        await fixture.Service.Review(fixture.Sheet.Id, "review");
        await fixture.Service.Review(fixture.Sheet.Id, "accept");
        Assert.True(fixture.Sheet.IsVerifiedTSO);
        Assert.NotNull(fixture.Sheet.ReviewRequestedAt);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
    }

    [Fact]
    public async Task MixedRolesCannotInitializeReviewerFieldsOnConsumerOnlyObject()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso);
        fixture.Access.ReviewerAccess = false;
        fixture.Sheet.IsVerifiedTSO = true;
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository, fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Sheet));
        fixture.Sheet.IsVerifiedTSO = false;
        fixture.Sheet.ReviewStartedAt = DateTime.UtcNow;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Sheet));
    }

    [Fact]
    public async Task CommissionMembershipGrantsTheSamePermissionsToCrudAndReviewApi()
    {
        var fixture = new Fixture(AdmissionCommitteeRoleNames.Member);
        Assert.False((await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanReview);
        fixture.AddCommissionMember();
        Assert.True((await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanReview);
        Assert.False((await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanFormAct);
        Assert.False((await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, Guid.NewGuid())).CanReview);
        fixture.Access.DenyGeneralAccess = true;
        fixture.Requirement.ProblemComment = "Исправить";
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository, fixture.Permissions, fixture.Service);
        await access.EnsureCanChange(fixture.Requirement);
        fixture.Sheet.ReviewRequestedAt = DateTime.UtcNow;
        await fixture.Service.Review(fixture.Sheet.Id, "review");
        fixture.Requirement.IsCompletedReq = true;
        await fixture.Service.Review(fixture.Sheet.Id, "accept");
        Assert.True(fixture.Sheet.IsVerifiedTSO);
        fixture.Members[0].AdmissionCommittee.Status = AdmissionCommitteeStatus.Editing;
        Assert.False((await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id, fixture.Sheet.Awp.Id)).CanReview);
    }

    [Fact]
    public async Task CommissionRemarksAreRestrictedToApprovedMembershipAndPeriod()
    {
        var fixture = new Fixture(AdmissionCommitteeRoleNames.Member);
        fixture.Requirement.ProblemComment = "Исправить";
        var provider = new WorkObjectRemarksProvider(fixture.Repository,
            new WorkspaceObjectScopeProvider(fixture.Repository, fixture.Access, fixture.Roles, fixture.Permissions),
            new TestPreparation(fixture.Sheet), new WorkObjectRequirementMapper(fixture.Repository), fixture.Permissions,
            new EvaluationSheetSelectionProvider(fixture.Repository));
        Assert.Empty(await provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "administration" }));
        fixture.AddCommissionMember();
        Assert.Single(await provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "administration" }));
        fixture.Members[0].AdmissionCommittee.Awp = new Awp();
        Assert.Empty(await provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "administration" }));
        fixture.Members[0].AdmissionCommittee.Awp = fixture.Sheet.Awp;
        fixture.Members[0].AdmissionCommittee.Organization = new Organization();
        fixture.Sheet.WorkObject.Organization.Hierarchy = new Organization();
        Assert.Empty(await provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "administration" }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "consumer" }));
    }

    [Fact]
    public async Task ConsumerCanResubmitAfterRemarksButCannotResetIndependentReview()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        fixture.Links.Add(new WorkObjectTsoOrganization { WorkObject = fixture.Sheet.WorkObject });
        fixture.Sheet.ReviewStartedAt = DateTime.UtcNow.AddDays(-2);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "submit"));
        fixture.Requirement.ProblemComment = "Исправить";
        await fixture.Service.Review(fixture.Sheet.Id, "submit");
        Assert.NotNull(fixture.Sheet.ReviewRequestedAt);
        Assert.Null(fixture.Sheet.ReviewStartedAt);
    }

    [Fact]
    public async Task ConsumerCannotAcceptEvenWhenTheObjectIsAccessible()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
        Assert.False(fixture.Committed);
    }

    [Fact]
    public async Task MixedRolesDoNotGrantReviewOfAnObjectOutsideReviewerScope()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso);
        fixture.Access.ReviewerAccess = false;
        var permissions = await fixture.Service.GetPermissions(fixture.Sheet.WorkObject.Id);
        Assert.True(permissions.CanSubmit);
        Assert.False(permissions.CanReview);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.Service.Review(fixture.Sheet.Id, "review"));
    }

    [Fact]
    public async Task SubmissionRequiresLinkedTso()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "submit"));
        fixture.Links.Add(new WorkObjectTsoOrganization { WorkObject = fixture.Sheet.WorkObject });
        await fixture.Service.Review(fixture.Sheet.Id, "submit");
        Assert.NotNull(fixture.Sheet.ReviewRequestedAt);
        Assert.True(fixture.Committed);
    }

    [Fact]
    public async Task TsoCannotBeginWithoutConsumerSubmission()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        var error = await Assert.ThrowsAsync<ArgumentException>(
            () => fixture.Service.Review(fixture.Sheet.Id, "review"));
        Assert.Equal("Перед началом проверки результат должен быть отправлен ТСО.", error.Message);
        Assert.Null(fixture.Sheet.ReviewRequestedAt);
        Assert.Null(fixture.Sheet.ReviewStartedAt);
        Assert.Null(fixture.Sheet.ActStatus);
        Assert.False(fixture.Committed);
    }

    [Fact]
    public async Task AcceptanceRequiresEnteredValueAndResolvedRemarks()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        fixture.Sheet.ReviewRequestedAt = DateTime.UtcNow;
        await fixture.Service.Review(fixture.Sheet.Id, "review");
        fixture.Requirement.HasEnteredValue = false;
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
        fixture.Requirement.HasEnteredValue = true;
        fixture.Requirement.ProblemComment = "Исправить";
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "accept"));
        fixture.Requirement.IsCompletedReq = true;
        await fixture.Service.Review(fixture.Sheet.Id, "accept");
        Assert.True(fixture.Requirement.IsVerified);
        Assert.True(fixture.Sheet.IsVerifiedTSO);
        Assert.Equal(0.834m, fixture.Sheet.ObjectReadinessIndex);
        Assert.Null(fixture.Sheet.ActStatus);
    }

    [Fact]
    public async Task NewSheetUsesLoadedObjectInsteadOfLookingForAnExistingSheet()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await access.EnsureCanChange(fixture.Sheet);
        Assert.Equal(Guid.Empty, fixture.Access.RelatedEntityId);
    }

    [Fact]
    public async Task DirectReviewChangesAlsoRequireObjectSpecificReviewerAccess()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso);
        fixture.Access.ReviewerAccess = false;
        fixture.Requirement.IsVerified = true;
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Requirement));
    }

    [Fact]
    public async Task DirectSubmissionRequiresLinkedTso()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        fixture.Sheet.ReviewRequestedAt = DateTime.UtcNow.AddYears(1);
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<ArgumentException>(() => access.EnsureCanChange(fixture.Sheet));
        fixture.Links.Add(new WorkObjectTsoOrganization { WorkObject = fixture.Sheet.WorkObject });
        await access.EnsureCanChange(fixture.Sheet);
        Assert.True(fixture.Sheet.ReviewRequestedAt < DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task CommissionRoleWithoutMembershipCannotChangeReviewResults()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, "Член комиссии");
        fixture.Requirement.IsVerified = true;
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Requirement));
        fixture.Members.Add(new AdmissionCommitteeEmployee
        {
            Employee = new Employee { Id = fixture.Access.EmployeeId },
            AdmissionCommittee = new AdmissionCommittee
            {
                Organization = fixture.Sheet.WorkObject.Organization,
                Awp = fixture.Sheet.Awp, Status = AdmissionCommitteeStatus.Approved
            }
        });
        await access.EnsureCanChange(fixture.Requirement);
        fixture.Members[0].AdmissionCommittee.Awp = new Awp();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Requirement));
    }

    [Fact]
    public async Task ConsumerCannotChangeRequirementWeight()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer);
        fixture.Requirement.SetLoadingState(ICSSoft.STORMNET.LoadingState.Loaded);
        fixture.Requirement.SetStatus(ICSSoft.STORMNET.ObjectStatus.UnAltered);
        fixture.Requirement.AddLoadedProperties(nameof(EvaluationSheetRequirement.Weight));
        fixture.Requirement.InitDataCopy();
        fixture.Requirement.Weight = 0.99m;
        fixture.Requirement.SetStatus(ICSSoft.STORMNET.ObjectStatus.Altered);
        Assert.Contains(nameof(EvaluationSheetRequirement.Weight), fixture.Requirement.GetAlteredPropertyNames(true));
        Assert.NotEqual(ICSSoft.STORMNET.ObjectStatus.Created, fixture.Requirement.GetStatus());
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => access.EnsureCanChange(fixture.Requirement));
    }

    [Fact]
    public async Task SignedSheetCannotBeSubmittedThroughDirectChanges()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        fixture.Sheet.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };
        fixture.Sheet.ReviewRequestedAt = DateTime.UtcNow;
        var access = new EvaluationSheetAccess(fixture.Access, null!, fixture.Repository,
            fixture.Permissions, fixture.Service);
        await Assert.ThrowsAsync<ArgumentException>(() => access.EnsureCanChange(fixture.Sheet));
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Review(fixture.Sheet.Id, "review"));
        Assert.False(EvaluationSheetReviewService.CanChangeReview(fixture.Sheet));
    }

    [Fact]
    public void StatusChangeOfOldSheetDoesNotReplaceNewVersion()
    {
        var workObject = new WorkObject();
        var oldSheet = new EvaluationSheet
        {
            WorkObject = workObject, RegDate = new DateTime(2026, 9, 1),
            ChangeStatusDate = new DateTime(2026, 9, 16)
        };
        var newSheet = new EvaluationSheet { WorkObject = workObject, RegDate = new DateTime(2026, 9, 15) };
        Assert.Same(newSheet, EvaluationSheetSelection.GetLatestByObject([oldSheet, newSheet])[workObject.Id]);
    }

    [Fact]
    public async Task WorkspaceLoadsObjectsUsingSelectedRoleInsteadOfUnionAccess()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso);
        var provider = new WorkspaceObjectScopeProvider(fixture.Repository, fixture.Access, fixture.Roles, fixture.Permissions);
        await provider.GetScope(new WorkspaceDashboardQuery { Workspace = "tso" });
        Assert.Equal(WorkObjectRoleNames.Tso, fixture.Access.SelectedRole);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => provider.GetScope(
            new WorkspaceDashboardQuery { Workspace = "administration" }));
    }

    [Fact]
    public async Task MixedRolesCannotExpandRemarksWorklistToConsumerWorkspace()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Consumer, WorkObjectRoleNames.Tso);
        var provider = new WorkObjectRemarksProvider(fixture.Repository, null!, null!, null!, fixture.Permissions,
            new EvaluationSheetSelectionProvider(fixture.Repository));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => provider.GetRemarks(
            new WorkspaceDashboardQuery { Workspace = "consumer" }));
    }

    [Fact]
    public async Task RemarksWithoutStateFiltersDoNotCalculateDashboardStatistics()
    {
        var fixture = new Fixture(WorkObjectRoleNames.Tso);
        fixture.Requirement.ProblemComment = "Исправить";
        var preparation = new TestPreparation(fixture.Sheet);
        var provider = new WorkObjectRemarksProvider(fixture.Repository,
            new WorkspaceObjectScopeProvider(fixture.Repository, fixture.Access, fixture.Roles, fixture.Permissions),
            preparation, new WorkObjectRequirementMapper(fixture.Repository), fixture.Permissions,
            new EvaluationSheetSelectionProvider(fixture.Repository));
        var remarks = await provider.GetRemarks(new WorkspaceDashboardQuery { Workspace = "tso" });
        Assert.Single(remarks);
        Assert.Equal(0, preparation.Calls);
        Assert.Equal(1, fixture.SheetReads);
        Assert.Equal(1, fixture.RequirementReads);
        Assert.Empty(await provider.GetRemarks(new WorkspaceDashboardQuery
        {
            Workspace = "tso", State = WorkObjectPreparationStates.InProgress
        }));
        Assert.Equal(1, preparation.Calls);
    }

    internal sealed class Fixture
    {
        public EvaluationSheet Sheet { get; } = new()
        {
            WorkObject = new WorkObject { Organization = new Organization() },
            Awp = new Awp(), ObjectReadinessIndex = 0.834m
        };
        public EvaluationSheetRequirement Requirement { get; }
        public List<WorkObjectTsoOrganization> Links { get; } = [];
        public List<AdmissionCommitteeEmployee> Members { get; } = [];
        public TestAccess Access { get; } = new();
        public TestRoles Roles { get; }
        public IRepository Repository { get; }
        public IUnitOfWork UnitOfWork { get; }
        public EvaluationSheetReviewService Service { get; }
        public EvaluationSheetPermissionProvider Permissions { get; }
        public bool Committed { get; private set; }
        public int SheetReads { get; private set; }
        public int RequirementReads { get; private set; }
        public int WorkObjectReads { get; private set; }
        public int SheetByIdReads { get; private set; }

        public Fixture(params string[] roles)
        {
            Roles = new TestRoles(roles);
            Access.Roles = roles;
            Requirement = new EvaluationSheetRequirement
            {
                EvaluationSheet = Sheet, ReqOrder = 6, IsCalculated = true,
                IsOptional = true, HasEnteredValue = true, CalculatedReq = 0
            };
            Sheet.Requirements.Add(Requirement);
            Access.Objects.Add(Sheet.WorkObject);
            Repository = DispatchProxy.Create<IRepository, RepositoryProxy>();
            ((RepositoryProxy)Repository).Handler = (method, args) => method.Name switch
            {
                "GetById" when method.GetGenericArguments()[0] == typeof(WorkObject)
                    => ReadWorkObject(),
                "GetById" when method.GetGenericArguments()[0] == typeof(EvaluationSheetRequirement)
                    => Task.FromResult<EvaluationSheetRequirement?>(Requirement),
                "GetById" => ReadSheet(),
                "Find" when method.GetGenericArguments()[0] == typeof(EvaluationSheetRequirement)
                    => ReadRequirements(),
                "Find" when method.GetGenericArguments()[0] == typeof(EvaluationSheet)
                    => ReadSheets(),
                "Find" when method.GetGenericArguments()[0] == typeof(Awp)
                    => Task.FromResult(new List<Awp> { Sheet.Awp }),
                "Find" when method.GetGenericArguments()[0] == typeof(AdmissionCommitteeEmployee)
                    => Task.FromResult(Members.Where(((Expression<Func<AdmissionCommitteeEmployee, bool>>)args![0]!).Compile()).ToList()),
                "Find" when method.GetGenericArguments()[0] == typeof(WorkObject)
                    => Task.FromResult(Access.Objects.Where(((Expression<Func<WorkObject, bool>>)args![0]!).Compile()).ToList()),
                "Find" => Task.FromResult(Links),
                "Any" => Task.FromResult(Members.Any(((Expression<Func<AdmissionCommitteeEmployee, bool>>)args![0]!).Compile())),
                "Update" => null,
                _ => throw new NotSupportedException(method.Name)
            };
            UnitOfWork = DispatchProxy.Create<IUnitOfWork, RepositoryProxy>();
            ((RepositoryProxy)(object)UnitOfWork).Handler = (method, _) => method.Name switch
            {
                "Commit" => Commit(),
                _ => throw new NotSupportedException(method.Name)
            };
            var users = new TestUsers(new User(), new Employee { Id = Access.EmployeeId });
            var memberships = new AdmissionCommitteeMembershipProvider(Repository, Roles, users);
            Permissions = new EvaluationSheetPermissionProvider(Repository, Access, Roles, memberships);
            Service = new EvaluationSheetReviewService(Repository, UnitOfWork, Permissions);
        }

        public void AddCommissionMember()
        {
            Members.Add(new AdmissionCommitteeEmployee
            {
                Employee = new Employee { Id = Access.EmployeeId },
                AdmissionCommittee = new AdmissionCommittee
                {
                    Organization = Sheet.WorkObject.Organization,
                    Awp = Sheet.Awp, Status = AdmissionCommitteeStatus.Approved
                }
            });
        }

        private Task<WorkObject?> ReadWorkObject()
        {
            WorkObjectReads++;
            return Task.FromResult<WorkObject?>(Sheet.WorkObject);
        }

        private Task<EvaluationSheet?> ReadSheet()
        {
            SheetByIdReads++;
            return Task.FromResult<EvaluationSheet?>(Sheet);
        }

        private Task<List<EvaluationSheet>> ReadSheets()
        {
            SheetReads++;
            return Task.FromResult(new List<EvaluationSheet> { Sheet });
        }

        private Task<List<EvaluationSheetRequirement>> ReadRequirements()
        {
            RequirementReads++;
            return Task.FromResult(new List<EvaluationSheetRequirement> { Requirement });
        }

        private Task Commit()
        {
            Committed = true;
            return Task.CompletedTask;
        }
    }

    public class RepositoryProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!, args);
    }

    internal sealed class TestRoles(string[] roles) : ICurrentRoleProvider
    {
        public ValueTask<List<string>> GetCurrentRoleNames() => ValueTask.FromResult(roles.ToList());
    }

    private sealed class TestUsers(IUser user, IEmployee employee) : ICurrentUserProvider
    {
        public Task<IUser?> GetCurrentUser() => Task.FromResult<IUser?>(user);
        public Task<IEmployee?> GetCurrentEmployee() => Task.FromResult<IEmployee?>(employee);
        public string? GetUserConnectionInfo() => null;
        public string? GetUserConnectionAddress() => null;
    }

    private sealed class TestPreparation(EvaluationSheet sheet) : IAwpPreparingAuditRecordProvider
    {
        public int Calls { get; private set; }
        public Task UpdatePartnerRecords(Guid awpId, Guid organizationId) => throw new NotSupportedException();
        public Task<IReadOnlyList<WorkObjectPreparationInfo>> GetObjectPreparationInfo(
            Guid? awpId, IReadOnlyCollection<Guid> workObjectIds)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<WorkObjectPreparationInfo>>(
                workObjectIds.Select(id => WorkObjectPreparationCalculator.Create(id, sheet, null, false)).ToList());
        }
    }

    internal sealed class TestAccess : IWorkObjectAccessProvider
    {
        public bool ReviewerAccess { get; set; } = true;
        public bool DenyGeneralAccess { get; set; }
        public string[] Roles { get; set; } = [];
        public int RoleAccessCalls { get; private set; }
        public int RoleIdCalls { get; private set; }
        public Guid EmployeeId { get; } = Guid.NewGuid();
        public string? SelectedRole { get; private set; }
        public List<WorkObject> Objects { get; } = [];
        public Guid? RelatedEntityId { get; private set; }
        public Task<bool> IsTechnicalAdministrator() => Task.FromResult(false);
        public Task<Guid?> GetCurrentEmployeeId() => Task.FromResult<Guid?>(EmployeeId);
        public Task<WorkObjectRoleAccess> GetRoleAccess(WorkObject workObject)
        {
            RoleAccessCalls++;
            return Task.FromResult(new WorkObjectRoleAccess(Roles.Contains(WorkObjectRoleNames.Consumer),
                Roles.Contains(WorkObjectRoleNames.Tso) && ReviewerAccess,
                Roles.Contains(WorkObjectRoleNames.Administration) && ReviewerAccess));
        }
        public Task<List<WorkObject>> GetAccessibleWorkObjects()
            => throw new NotSupportedException();
        public Task<List<WorkObject>> GetAccessibleWorkObjects(string role)
        {
            SelectedRole = role;
            return Task.FromResult(Objects);
        }
        public Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(IEnumerable<Guid> ids)
            => Task.FromResult<IReadOnlySet<Guid>>(ids.ToHashSet());
        public Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(IEnumerable<Guid> ids, string role)
        {
            RoleIdCalls++;
            return Task.FromResult<IReadOnlySet<Guid>>(role == WorkObjectRoleNames.Consumer || ReviewerAccess ? ids.ToHashSet() : new HashSet<Guid>());
        }
        public async Task<bool> CanAccess(IEnumerable<Guid> ids, string role)
        {
            var requested = ids.Where(id => id != Guid.Empty).Distinct().ToList();
            if (requested.Count == 0)
            {
                return false;
            }

            var allowed = await GetAccessibleWorkObjectIds(requested, role);
            return requested.All(allowed.Contains);
        }

        public async Task EnsureCanAccess(IEnumerable<Guid> ids, string role, bool allowEmpty = false)
        {
            var requested = ids.Where(id => id != Guid.Empty).Distinct().ToList();
            var allowed = await GetAccessibleWorkObjectIds(requested, role);
            if ((!allowEmpty && requested.Count == 0)
                || requested.Any(id => !allowed.Contains(id)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task EnsureCanAccess(
            Guid? loadedId,
            Guid entityId,
            Func<Task<Guid?>> storedId,
            string role)
        {
            if (DenyGeneralAccess)
            {
                throw new UnauthorizedAccessException();
            }

            var ids = new HashSet<Guid>();
            if (loadedId.HasValue)
            {
                ids.Add(loadedId.Value);
            }

            if (entityId != Guid.Empty)
            {
                var stored = await storedId();
                if (!stored.HasValue)
                {
                    throw new UnauthorizedAccessException();
                }

                ids.Add(stored.Value);
            }

            RelatedEntityId = entityId;
            await EnsureCanAccess(ids, role);
        }

        public Task EnsureCanAccess(Guid id) => Task.CompletedTask;
        public Task EnsureCanAccess(IEnumerable<Guid> ids, bool allowEmpty = false) => Task.CompletedTask;
        public Task EnsureCanAccess(Guid? loadedId, Guid entityId, Func<Task<Guid?>> storedId)
        {
            if (DenyGeneralAccess)
            {
                throw new UnauthorizedAccessException();
            }
            RelatedEntityId = entityId;
            return Task.CompletedTask;
        }
        public Task EnsureCanCreateWorkObject(Guid organizationId) => Task.CompletedTask;
        public Task EnsureTechnicalAdministrator() => throw new UnauthorizedAccessException();
    }
}
