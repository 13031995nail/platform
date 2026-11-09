namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;

public sealed class AwpPreparingAuditRecordBS(AwpPreparingAuditRecordAccess access)
    : EntityBS<AwpPreparingAuditRecord>
{
    public DataObject[] OnUpdateAwpPreparingAuditRecord(
        AwpPreparingAuditRecord updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpPreparingAuditRecord entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
