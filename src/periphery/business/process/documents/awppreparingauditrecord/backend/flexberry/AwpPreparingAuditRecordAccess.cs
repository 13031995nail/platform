namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using Tmobis.Business.Common.Address;
using Tmobis.Core.Repositories;

public sealed class AwpPreparingAuditRecordAccess(
    IWorkObjectAccessProvider workObjectAccessProvider,
    IRepository repository)
{
    public async Task EnsureCanChange(AwpPreparingAuditRecord record)
    {
        await workObjectAccessProvider.EnsureCanAccess(
            record.WorkObject?.Id,
            record.Id,
            async () => (await repository.Find<AwpPreparingAuditRecord>(item =>
                item.Id == record.Id)).FirstOrDefault()?.WorkObject?.Id);
    }
}

public sealed class AwpPreparingAuditRecordReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider)
    : IODataReadRestrictionProvider
{
    public bool Supports(Type entityType)
        => entityType == typeof(AwpPreparingAuditRecord);

    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
        => ODataReadRestriction.ForProperty(
            await readLimitProvider.GetReadLimit(nameof(AwpPreparingAuditRecord.WorkObject)),
            nameof(AwpPreparingAuditRecord.WorkObject));
}
