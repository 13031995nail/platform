namespace Tmobis.Business.Process.Documents.Tests;

using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Repositories;
using Xunit;

public sealed class PartnerReadinessTests
{
    [Theory]
    [InlineData("0.79", PartnerReadinessLevel.NotReady)]
    [InlineData("0.8", PartnerReadinessLevel.ConditionReady)]
    [InlineData("0.89", PartnerReadinessLevel.ConditionReady)]
    [InlineData("0.9", PartnerReadinessLevel.Ready)]
    [InlineData("1", PartnerReadinessLevel.Ready)]
    public async Task ExistingReadinessBoundariesArePreserved(string value, PartnerReadinessLevel expected)
    {
        var index = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        var record = new AwpPreparingAuditRecord { IndexReadiness = index };
        var repository = CreateRepository([record]);
        var provider = new AwpPreparingAuditRecordProvider(repository,
            NullLogger<AwpPreparingAuditRecordProvider>.Instance, new EvaluationSheetSelectionProvider(repository));

        await provider.UpdatePartnerRecords(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(expected, record.PartnerReadiness);
        Assert.Equal(index, record.IndexPartnerReadiness);
    }

    [Fact]
    public async Task EmptyJournalDoesNotFailOrUpdateRecords()
    {
        var repository = CreateRepository([]);
        var provider = new AwpPreparingAuditRecordProvider(repository,
            NullLogger<AwpPreparingAuditRecordProvider>.Instance, new EvaluationSheetSelectionProvider(repository));
        await provider.UpdatePartnerRecords(Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(0, ((RepositoryProxy)repository).Updates);
    }

    [Fact]
    public async Task RepositoryFailureIsNotReportedAsSuccess()
    {
        var repository = CreateRepository([]);
        ((RepositoryProxy)repository).Failure = new InvalidOperationException("Ошибка репозитория");
        var provider = new AwpPreparingAuditRecordProvider(repository,
            NullLogger<AwpPreparingAuditRecordProvider>.Instance, new EvaluationSheetSelectionProvider(repository));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.UpdatePartnerRecords(Guid.NewGuid(), Guid.NewGuid()));
    }

    private static IRepository CreateRepository(List<AwpPreparingAuditRecord> records)
    {
        var repository = DispatchProxy.Create<IRepository, RepositoryProxy>();
        ((RepositoryProxy)repository).Records = records;
        return repository;
    }

    public class RepositoryProxy : DispatchProxy
    {
        public List<AwpPreparingAuditRecord> Records { get; set; } = [];
        public Exception? Failure { get; set; }
        public int Updates { get; private set; }

        protected override object? Invoke(MethodInfo? method, object?[]? args)
        {
            if (Failure != null) throw Failure;
            if (method!.Name == "Find") return Task.FromResult(Records);
            if (method.Name == "Update")
            {
                Updates++;
                return null;
            }
            throw new NotSupportedException(method.Name);
        }
    }
}
