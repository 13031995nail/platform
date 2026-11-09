namespace Tmobis.Business.Process.Documents.Tests;

using System.Linq.Expressions;
using System.Reflection;
using ICSSoft.STORMNET;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;
using Employee = Tmobis.Business.Core.Flexberry.Models.Employee;
using UnauthorizedAccessException = System.UnauthorizedAccessException;
using Xunit;

public sealed class ActSigningCancellationTests
{
    [Theory]
    [InlineData("readiness-act-sign")]
    [InlineData("awp-readiness-passport-sign")]
    [InlineData("awp-assessing-program-sign")]
    [InlineData("evaluation-sheet-sign")]
    public async Task BaseSignerClearsAllSignaturesAndStatusInOneAtomicCommit(string model)
    {
        var fixture = new Fixture(model);
        await fixture.Service.Cancel(model, fixture.Document.Id);
        Assert.Equal(IActStatus.Keys.Editing, fixture.Document.ActStatus.Id);
        Assert.All(fixture.Signs, sign =>
        {
            Assert.Null(sign.SignDate);
            Assert.Null(sign.Signature);
            Assert.NotNull(sign.Employee);
            Assert.NotNull(sign.SignRole);
        });
        Assert.Equal(1, fixture.Commits);
        Assert.Equal(4, fixture.Pending.Count);
        Assert.False(ActSigningCancellationScope.AllowsDocument(fixture.Document));
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.Cancel(model, fixture.Document.Id));
        Assert.Equal(1, fixture.Commits);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MissingOrOrdinarySignerCannotCancel(bool ordinarySigner)
    {
        var fixture = new Fixture("readiness-act-sign");
        fixture.Access.CurrentEmployeeId = ordinarySigner ? fixture.Signs[1].Employee!.Id : Guid.NewGuid();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.Service.Cancel("readiness-act-sign", fixture.Document.Id));
        fixture.AssertUnchanged();
        Assert.Empty(fixture.Pending);
    }

    [Fact]
    public async Task InaccessibleLinkedObjectPreventsAnyWrite()
    {
        var fixture = new Fixture("readiness-act-sign");
        fixture.Entities.Add(new RedinessActObject
        {
            Act = (RedinessAct)fixture.Document, WorkObject = new WorkObject(),
        });
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.Service.Cancel("readiness-act-sign", fixture.Document.Id));
        fixture.AssertUnchanged();
        Assert.Empty(fixture.Pending);
    }

    [Fact]
    public async Task CommitFailureRestoresLoadedValuesAndRevokesCancellationPermission()
    {
        var fixture = new Fixture("readiness-act-sign") { FailCommit = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Service.Cancel("readiness-act-sign", fixture.Document.Id));
        fixture.AssertUnchanged();
        Assert.False(ActSigningCancellationScope.AllowsDocument(fixture.Document));
        var otherSign = (ReadinessActSign)fixture.Signs[1];
        otherSign.SignDate = null;
        otherSign.Signature = null;
        Assert.False(ActSigningCancellationScope.AllowsSign(otherSign));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.ActAccess.EnsureCanChange(otherSign));
    }

    [Fact]
    public async Task DirectPatchOfAnotherSignatureRemainsForbidden()
    {
        var fixture = new Fixture("readiness-act-sign");
        var sign = (ReadinessActSign)fixture.Signs[1];
        sign.SignDate = null;
        sign.Signature = null;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.ActAccess.EnsureCanChange(sign));
        Assert.Equal(0, fixture.Commits);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NonSigningDocumentCannotBeCancelled(bool approved)
    {
        var fixture = new Fixture("readiness-act-sign");
        fixture.Document.ActStatus = new ActStatus { Id = approved ? IActStatus.Keys.Approved : IActStatus.Keys.Signed };
        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Service.Cancel("readiness-act-sign", fixture.Document.Id));
        Assert.All(fixture.Signs, sign => Assert.NotNull(sign.SignDate));
        Assert.Empty(fixture.Pending);
    }

    [Fact]
    public async Task CancellationScopeCannotPermitAssignmentChangesOrOtherLoadedInstances()
    {
        var fixture = new Fixture("readiness-act-sign");
        var sign = (ReadinessActSign)fixture.Signs[1];
        sign.SignDate = null;
        sign.Signature = null;
        using (ActSigningCancellationScope.Enter(fixture.Document, fixture.Signs))
        {
            Assert.True(ActSigningCancellationScope.AllowsSign(sign));
            Assert.False(ActSigningCancellationScope.AllowsSign(new ReadinessActSign { Id = sign.Id }));
            sign.Employee = new Employee();
            Assert.False(ActSigningCancellationScope.AllowsSign(sign));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.ActAccess.EnsureCanChange(sign));
        }
        Assert.False(ActSigningCancellationScope.AllowsSign(sign));
        Assert.Empty(fixture.Pending);
    }

    [Fact]
    public void CancellationScopeCannotPermitDocumentEditsAlongsideStatusChange()
    {
        var fixture = new Fixture("readiness-act-sign");
        var act = (RedinessAct)fixture.Document;
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Editing };
        using (ActSigningCancellationScope.Enter(act, fixture.Signs))
        {
            Assert.True(ActSigningCancellationScope.AllowsDocument(act));
            act.ActNumber = "changed";
            Assert.False(ActSigningCancellationScope.AllowsDocument(act));
        }
        Assert.False(ActSigningCancellationScope.AllowsDocument(act));
    }

    private sealed class Fixture
    {
        public List<IEntity> Entities { get; } = [];
        public List<IEntity> Pending { get; private set; } = [];
        public TestWorkObjectAccessProvider Access { get; } = new();
        public BaseActBase Document { get; }
        public List<ActSign> Signs { get; } = [];
        public RedinessActAccess ActAccess { get; }
        public ActSigningCancellationService Service { get; }
        public int Commits { get; private set; }
        public bool FailCommit { get; init; }

        public Fixture(string model)
        {
            var repository = new InMemoryDocumentRepository(
                Entities,
                onUpdate: models => Pending = models,
                onAtomicCommit: () =>
                {
                    Assert.Equal(4, Pending.Count);
                    Assert.All(Pending.OfType<ActSign>(), sign => Assert.Null(sign.SignDate));
                    if (FailCommit) throw new InvalidOperationException("Simulated database failure");
                    Commits++;
                });
            var organization = new Organization();
            var workObject = new WorkObject { Organization = organization, Actuality = true };
            Entities.Add(workObject);
            ((HashSet<Guid>)Access.GenericIds).Add(workObject.Id);
            var awp = new Awp();
            var act = new RedinessAct
            {
                Organization = organization,
                Awp = awp,
                ActStatus = new ActStatus { Id = IActStatus.Keys.Signed },
            };
            var baseRole = ISignRole.Keys.CommissionChair;
            Document = model switch
            {
                "awp-readiness-passport-sign" => new AwpReadinessPassport
                {
                    Organization = organization,
                    Awp = awp,
                    ReadinessAct = act,
                },
                "awp-assessing-program-sign" => new AwpAssessingProgram { Organization = organization },
                "evaluation-sheet-sign" => new EvaluationSheet { WorkObject = workObject },
                _ => act,
            };
            if (Document is AwpReadinessPassport) baseRole = ISignRole.Keys.CommissionCreator;
            if (Document is EvaluationSheet) baseRole = ISignRole.Keys.CommissionMember;
            Entities.Add(act);
            if (!ReferenceEquals(act, Document)) Entities.Add(Document);
            Entities.Add(new RedinessActObject { Act = act, WorkObject = workObject });
            if (Document is AwpReadinessPassport passport)
                Entities.Add(new AwpReadinessPassportObject { Act = passport, WorkObject = workObject });
            if (Document is AwpAssessingProgram program)
                Entities.Add(new AwpAssessingProgramObject { Program = program, WorkObject = workObject });
            Entities.Add(new ActStatus { Id = IActStatus.Keys.Editing });
            Document.ActStatus = new ActStatus { Id = IActStatus.Keys.Signing };
            MarkStored(Document);
            for (var index = 0; index < 3; index++)
            {
                ActSign sign = Document switch
                {
                    AwpReadinessPassport value => new AwpReadinessPassportSign { Act = value },
                    AwpAssessingProgram value => new AwpAssessingProgramSign { Act = value },
                    EvaluationSheet value => new EvaluationSheetSign { Act = value },
                    _ => new ReadinessActSign { Act = act },
                };
                sign.Employee = new Employee();
                sign.SignRole = new SignRole { Id = index == 0 ? baseRole : ISignRole.Keys.CommissionMember };
                sign.SignDate = DateTime.UtcNow;
                sign.Signature = "signature";
                MarkStored(sign);
                Signs.Add(sign);
                Entities.Add(sign);
            }
            Access.CurrentEmployeeId = Signs[0].Employee!.Id;
            var validator = new ActSignAccessValidator(repository);
            ActAccess = new RedinessActAccess(Access, validator, repository);
            var users = new BoundaryUsers();
            users.Employee.Id = Signs[0].Employee!.Id;
            var membership = new AdmissionCommitteeMembershipProvider(
                repository, new BoundaryRoles(), users);
            var permission = new EvaluationSheetPermissionProvider(repository, Access, new BoundaryRoles(), membership);
            Service = new ActSigningCancellationService(repository, Access, ActAccess,
                new AwpReadinessPassportAccess(Access, validator, repository),
                new AwpAssessingProgramAccess(Access, validator, repository),
                new EvaluationSheetAccess(Access, validator, repository, permission,
                    new EvaluationSheetReviewService(repository, (IUnitOfWork)repository, permission)));
        }

        public void AssertUnchanged()
        {
            Assert.Equal(IActStatus.Keys.Signing, Document.ActStatus.Id);
            Assert.All(Signs, sign =>
            {
                Assert.NotNull(sign.SignDate);
                Assert.Equal("signature", sign.Signature);
            });
            Assert.Equal(0, Commits);
        }

        private static void MarkStored(DataObject entity)
        {
            var names = new[] { "Act", "ActStatus", "Organization", "Awp", "ReadinessAct", "WorkObject",
                "Employee", "SignRole", "SignDate", "Signature", "Objects" };
            var properties = entity.GetType().GetProperties().Where(property => names.Contains(property.Name))
                .ToDictionary(property => property, property => property.GetValue(entity));
            entity.SetExistObjectPrimaryKey(entity.__PrimaryKey);
            foreach (var (property, value) in properties) property.SetValue(entity, value);
            entity.AddLoadedProperties(properties.Keys.Select(property => property.Name).ToArray());
            entity.InitDataCopy();
            foreach (var (property, value) in properties) property.SetValue(entity.GetDataCopy(), value);
            entity.SetStatus(ObjectStatus.UnAltered);
        }
    }
}
