namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;

public sealed class AwpReadinessPassportObjectBS(AwpReadinessPassportAccess access)
    : EntityBS<AwpReadinessPassportObject>
{
    public DataObject[] OnUpdateAwpReadinessPassportObject(
        AwpReadinessPassportObject updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpReadinessPassportObject entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}

public sealed class AwpReadinessPassportSignBS(AwpReadinessPassportAccess access)
    : EntityBS<AwpReadinessPassportSign>
{
    public DataObject[] OnUpdateAwpReadinessPassportSign(
        AwpReadinessPassportSign updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpReadinessPassportSign entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
