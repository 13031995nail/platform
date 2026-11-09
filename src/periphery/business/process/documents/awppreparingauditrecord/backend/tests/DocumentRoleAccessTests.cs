namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using Xunit;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class DocumentRoleAccessTests
{
    [Fact]
    public async Task TsoCannotCreateReadinessActForForeignObject()
    {
        var fixture = new Fixture();
        fixture.Access.RoleIds[WorkObjectRoleNames.Tso] =
            new HashSet<Guid> { Guid.NewGuid() };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    [Fact]
    public async Task ConsumerScopeCannotCreateReadinessAct()
    {
        var fixture = new Fixture();
        fixture.Access.RoleIds[WorkObjectRoleNames.Consumer] =
            new HashSet<Guid> { fixture.WorkObject.Id };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    [Fact]
    public async Task TsoScopeCannotCreateReadinessPassport()
    {
        var fixture = new Fixture();
        fixture.PreparePassportSourceAct();
        fixture.Access.RoleIds[WorkObjectRoleNames.Tso] =
            new HashSet<Guid> { fixture.WorkObject.Id };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreatePassportAccess().EnsureCanChange(fixture.Passport));
    }

    [Fact]
    public async Task AdministrationCanCreateReadinessPassportForOwnObject()
    {
        var fixture = new Fixture();
        fixture.PreparePassportSourceAct();
        fixture.Access.RoleIds[WorkObjectRoleNames.Administration] =
            new HashSet<Guid> { fixture.WorkObject.Id };

        await fixture.CreatePassportAccess().EnsureCanChange(fixture.Passport);
    }

    [Fact]
    public async Task AssignedSignerCannotBundleActChangesWithoutTsoScope()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: true);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    [Fact]
    public async Task AssignedSignerCanMakeIsolatedStatusChangeInObjectScope()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: false);
        Assert.True(
            Base.Flexberry.BaseActSigningChangeDetector
                .IsIsolatedSigningStatusChange(fixture.Act),
            string.Join(',', fixture.Act.GetAlteredPropertyNames()));

        await fixture.CreateActAccess().EnsureCanChange(fixture.Act);
    }

    [Fact]
    public async Task AssignedSignerCannotCompleteActWithMissingSignatures()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: false);
        fixture.Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    [Fact]
    public async Task SingleSignerCanCompleteActAfterSavingSignature()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: false);
        fixture.SignDate = DateTime.UtcNow;
        fixture.Signature = "signature";
        fixture.Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        await fixture.CreateActAccess().EnsureCanChange(fixture.Act);
    }

    [Fact]
    public async Task AssignedSignerCannotCompleteActWithOnlySignatureDate()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: false);
        fixture.SignDate = DateTime.UtcNow;
        fixture.Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    [Fact]
    public async Task RegularAssignedSignerCannotStartActSigning()
    {
        var fixture = new Fixture();
        fixture.PrepareActSigningChange(includeDocumentChange: false);
        fixture.SignRoleId = ISignRole.Keys.CommissionMember;

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.CreateActAccess().EnsureCanChange(fixture.Act));
    }

    private sealed class Fixture
    {
        private readonly IRepository repository;

        public Organization Organization { get; } = new();

        public WorkObject WorkObject { get; }

        public RedinessAct Act { get; }

        public AwpReadinessPassport Passport { get; }

        public Awp Awp { get; } = new();

        public TestWorkObjectAccessProvider Access { get; }

        public DateTime? SignDate { get; set; }

        public string? Signature { get; set; }

        public Guid SignRoleId { get; set; } = ISignRole.Keys.CommissionChair;

        public Fixture()
        {
            WorkObject = new WorkObject
            {
                Actuality = true,
                Organization = Organization,
            };
            Act = new RedinessAct
            {
                Organization = Organization,
            };
            var actObject = new RedinessActObject
            {
                Act = Act,
                WorkObject = WorkObject,
            };
            Act.Objects.Add(actObject);
            Passport = new AwpReadinessPassport
            {
                Organization = Organization,
                ReadinessAct = Act,
            };
            Access = new TestWorkObjectAccessProvider([WorkObject.Id]);
            repository = CreateRepository(WorkObject, actObject);
        }

        public RedinessActAccess CreateActAccess()
            => new(
                Access,
                new ActSignAccessValidator(repository),
                repository);

        public AwpReadinessPassportAccess CreatePassportAccess()
            => new(
                Access,
                new ActSignAccessValidator(repository),
                repository);

        public void PreparePassportSourceAct()
        {
            Act.Awp = Awp;
            Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };
            Act.SetExistObjectPrimaryKey(Act.__PrimaryKey);
            Act.AddLoadedProperties(
                nameof(RedinessAct.Organization),
                nameof(RedinessAct.Awp),
                nameof(RedinessAct.Objects),
                nameof(RedinessAct.ActStatus));
            Act.InitDataCopy();
            var storedAct = Act.GetDataCopy() as RedinessAct
                ?? throw new InvalidOperationException("Stored readiness act is not configured.");
            storedAct.Organization = Organization;
            storedAct.Awp = Awp;
            storedAct.Objects = Act.Objects;
            storedAct.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };
            Act.SetStatus(ObjectStatus.UnAltered);

            Passport.Awp = Awp;
            Passport.SetStatus(ObjectStatus.Created);
        }

        public void PrepareActSigningChange(bool includeDocumentChange)
        {
            Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Editing };
            Act.ActStatus.SetExistObjectPrimaryKey(IActStatus.Keys.Editing);
            Act.ActNumber = "initial";
            Act.SetExistObjectPrimaryKey(Guid.NewGuid());
            Act.AddLoadedProperties(
                nameof(RedinessAct.Organization),
                nameof(RedinessAct.Objects),
                nameof(RedinessAct.ActStatus),
                nameof(RedinessAct.ActNumber));
            Act.InitDataCopy();
            Act.Organization = Organization;
            Act.Objects = new(Act);
            Act.Objects.Add(new RedinessActObject
            {
                Act = Act,
                WorkObject = WorkObject,
            });
            var storedAct = Act.GetDataCopy() as RedinessAct
                ?? throw new InvalidOperationException(
                    "Stored readiness act is not configured.");
            storedAct.Organization = Act.Organization;
            storedAct.Objects = Act.Objects;
            storedAct.ActStatus = new ActStatus { Id = IActStatus.Keys.Editing };
            Act.SetStatus(ObjectStatus.UnAltered);
            Act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signing };
            if (includeDocumentChange)
            {
                Act.ActNumber = "changed";
            }

            Act.SetStatus(ObjectStatus.Altered);
            Access.CurrentEmployeeId = Guid.NewGuid();
        }

        private IRepository CreateRepository(
            WorkObject workObject,
            RedinessActObject actObject)
        {
            return TestRepository.Create((method, _) =>
            {
                var entityType = method.GetGenericArguments().FirstOrDefault();
                if (method.Name == "Find" && entityType == typeof(WorkObject))
                {
                    return Task.FromResult(new List<WorkObject> { workObject });
                }

                if (method.Name == "Find" && entityType == typeof(RedinessActObject))
                {
                    return Task.FromResult(new List<RedinessActObject> { actObject });
                }

                if (method.Name == "Find" && entityType == typeof(RedinessAct))
                {
                    var storedAct = actObject.Act?.GetDataCopy() as RedinessAct
                        ?? throw new InvalidOperationException(
                            "Stored readiness act is not configured.");
                    return Task.FromResult(new List<RedinessAct>
                    {
                        storedAct,
                    });
                }

                if (method.Name == "Find" && entityType == typeof(ReadinessActSign))
                {
                    return Task.FromResult(new List<ReadinessActSign>
                    {
                        new()
                        {
                            Act = Act,
                            Employee = new Employee
                            {
                                Id = Access.CurrentEmployeeId!.Value,
                            },
                            SignRole = new SignRole { Id = SignRoleId },
                            SignDate = SignDate,
                            Signature = Signature,
                        },
                    });
                }

                if (method.Name == "Any")
                {
                    return Task.FromResult(true);
                }

                throw new NotSupportedException(method.Name);
            });
        }
    }
}
