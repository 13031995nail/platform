namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;
using Xunit;

public sealed class ActSignAccessValidatorTests
{
    [Fact]
    public async Task MissingCurrentEmployeeCannotSign()
    {
        var validator = new ActSignAccessValidator(CreateRepository(true));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            validator.EnsureCurrentEmployeeAssigned<ReadinessActSign>(
                _ => _ => true,
                () => Task.FromResult<Guid?>(null)));
    }

    [Fact]
    public async Task EmployeeWithoutAssignmentCannotSign()
    {
        var validator = new ActSignAccessValidator(CreateRepository(false));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            validator.EnsureCurrentEmployeeAssigned<ReadinessActSign>(
                _ => _ => true,
                () => Task.FromResult<Guid?>(Guid.NewGuid())));
    }

    [Fact]
    public async Task AssignedEmployeeCanSign()
    {
        var validator = new ActSignAccessValidator(CreateRepository(true));

        await validator.EnsureCurrentEmployeeAssigned<ReadinessActSign>(
            _ => _ => true,
            () => Task.FromResult<Guid?>(Guid.NewGuid()));
    }

    [Fact]
    public async Task AssignedSignerCannotSaveSignDateWithoutSignature()
    {
        var employeeId = Guid.NewGuid();
        var sign = new ReadinessActSign { SignDate = DateTime.UtcNow };
        sign.SetStatus(ObjectStatus.Altered);
        var validator = new ActSignAccessValidator(CreateRepository(true));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => validator.EnsureCanSign(
            sign,
            nameof(ReadinessActSign.Act),
            () => Task.FromResult(false),
            () => Task.FromResult<Guid?>(employeeId)));
    }

    [Fact]
    public async Task AssignedSignerCannotSaveSignatureWithoutSignDate()
    {
        var employeeId = Guid.NewGuid();
        var sign = new ReadinessActSign { Signature = "signature" };
        sign.SetStatus(ObjectStatus.Altered);
        var validator = new ActSignAccessValidator(CreateRepository(true));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => validator.EnsureCanSign(
            sign,
            nameof(ReadinessActSign.Act),
            () => Task.FromResult(false),
            () => Task.FromResult<Guid?>(employeeId)));
    }

    [Theory]
    [InlineData(true, "", false)]
    [InlineData(true, "   ", false)]
    [InlineData(false, "signature", false)]
    [InlineData(true, "signature", true)]
    public async Task GetSigningStatusRequiresSignatureAndDate(
        bool hasSignDate,
        string signature,
        bool expectedAllSigned)
    {
        var employeeId = Guid.NewGuid();
        var sign = new ReadinessActSign
        {
            Employee = new() { Id = employeeId },
            SignDate = hasSignDate ? DateTime.UtcNow : null,
            Signature = signature,
        };
        var validator = new ActSignAccessValidator(CreateRepository(true, [sign]));

        var status = await validator.GetSigningStatus<ReadinessActSign>(
            _ => true,
            () => Task.FromResult<Guid?>(employeeId),
            Guid.NewGuid());

        Assert.Equal(expectedAllSigned, status.AllSigned);
    }

    private static IRepository CreateRepository(
        bool isAssigned,
        List<ReadinessActSign>? signs = null)
        => TestRepository.Create((method, _) => method.Name switch
        {
            "Any" => Task.FromResult(isAssigned),
            "Find" => Task.FromResult(signs ?? []),
            _ => throw new NotSupportedException(method.Name),
        });
}
