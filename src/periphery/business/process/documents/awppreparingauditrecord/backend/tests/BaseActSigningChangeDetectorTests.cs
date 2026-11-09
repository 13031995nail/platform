namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Xunit;

public sealed class BaseActSigningChangeDetectorTests
{
    [Fact]
    public void BundledDocumentChangesAreNotTreatedAsIsolatedSigning()
    {
        var act = CreateStoredAct();
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signing };
        act.ActNumber = "changed-together-with-status";
        act.SetStatus(ObjectStatus.Altered);

        Assert.True(
            BaseActSigningChangeDetector.IsSigningStatusChange(act),
            Describe(act));
        Assert.False(
            BaseActSigningChangeDetector.IsIsolatedSigningStatusChange(act));
    }

    [Fact]
    public void IsolatedSigningStatusChangeIsAllowedForFurtherSignerCheck()
    {
        var act = CreateStoredAct();
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signing };
        act.SetStatus(ObjectStatus.Altered);

        Assert.True(
            BaseActSigningChangeDetector.IsSigningStatusChange(act),
            Describe(act));
        Assert.True(
            BaseActSigningChangeDetector.IsIsolatedSigningStatusChange(act));
    }

    private static RedinessAct CreateStoredAct()
    {
        var act = new RedinessAct
        {
            ActStatus = new ActStatus { Id = IActStatus.Keys.Editing },
            ActNumber = "initial",
        };
        act.SetExistObjectPrimaryKey(Guid.NewGuid());
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Editing };
        act.ActStatus.SetExistObjectPrimaryKey(IActStatus.Keys.Editing);
        act.AddLoadedProperties(nameof(act.ActStatus), nameof(act.ActNumber));
        act.InitDataCopy();
        ((RedinessAct)act.GetDataCopy()).ActStatus = act.ActStatus;
        act.SetStatus(ObjectStatus.UnAltered);
        return act;
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void ProgramApprovalRequiresAllSignaturesAndBaseSigner(bool allSigned, bool baseSigner)
    {
        var program = new AwpAssessingProgram
        {
            ActStatus = new ActStatus { Id = IActStatus.Keys.Approved },
        };

        Assert.Throws<System.UnauthorizedAccessException>(() =>
            BaseActBaseBS.EnsureValidStatusTransition(
                program, IActStatus.Keys.Signed, allSigned, baseSigner, allowApproval: true));
    }

    [Fact]
    public void ProgramApprovalRequiresPreviouslySignedStatus()
    {
        var program = new AwpAssessingProgram
        {
            ActStatus = new ActStatus { Id = IActStatus.Keys.Approved },
        };

        Assert.Throws<System.UnauthorizedAccessException>(() =>
            BaseActBaseBS.EnsureValidStatusTransition(
                program, IActStatus.Keys.Editing, true, true, allowApproval: true));
        BaseActBaseBS.EnsureValidStatusTransition(
            program, IActStatus.Keys.Signed, true, true, allowApproval: true);
    }

    [Fact]
    public void PassportDoesNotGainProgramApprovalTransition()
    {
        var passport = new AwpReadinessPassport
        {
            ActStatus = new ActStatus { Id = IActStatus.Keys.Approved },
        };

        Assert.Throws<System.UnauthorizedAccessException>(() =>
            BaseActBaseBS.EnsureValidStatusTransition(
                passport, IActStatus.Keys.Signed, true, true));
    }

    [Fact]
    public void ApprovedProgramCannotBeReopenedBySigningStatusPatch()
    {
        var program = new AwpAssessingProgram
        {
            ActStatus = new ActStatus { Id = IActStatus.Keys.Signing },
        };

        Assert.Throws<System.UnauthorizedAccessException>(() =>
            BaseActBaseBS.EnsureValidStatusTransition(
                program, IActStatus.Keys.Approved, true, true));
    }

    [Fact]
    public void CachedAlteredDocumentUsesOriginalStatus()
    {
        var act = CreateStoredAct();
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signing };
        act.SetStatus(ObjectStatus.Altered);

        Assert.Equal(IActStatus.Keys.Editing,
            BaseActBaseBS.GetStoredStatusId(act, act));
    }

    private static string Describe(RedinessAct act)
        => $"status={act.GetStatus()}, actStatusAltered="
            + $"{act.IsAlteredProperty(nameof(act.ActStatus))}, altered="
            + string.Join(',', act.GetAlteredPropertyNames());
}
