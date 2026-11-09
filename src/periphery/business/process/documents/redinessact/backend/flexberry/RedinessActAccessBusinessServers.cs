namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;

public sealed class RedinessActObjectBS(RedinessActAccess access)
    : EntityBS<RedinessActObject>
{
    public DataObject[] OnUpdateRedinessActObject(RedinessActObject updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        RedinessActObject entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}

public sealed class ReadinessActSignBS(RedinessActAccess access)
    : EntityBS<ReadinessActSign>
{
    public DataObject[] OnUpdateReadinessActSign(ReadinessActSign updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        ReadinessActSign entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
