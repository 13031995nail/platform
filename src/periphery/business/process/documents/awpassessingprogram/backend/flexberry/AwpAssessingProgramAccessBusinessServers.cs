namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;

public sealed class AwpAssessingProgramObjectBS(AwpAssessingProgramAccess access)
    : EntityBS<AwpAssessingProgramObject>
{
    public DataObject[] OnUpdateAwpAssessingProgramObject(
        AwpAssessingProgramObject updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpAssessingProgramObject entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}

public sealed class AwpAssessingProgramSignBS(AwpAssessingProgramAccess access)
    : EntityBS<AwpAssessingProgramSign>
{
    public DataObject[] OnUpdateAwpAssessingProgramSign(
        AwpAssessingProgramSign updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpAssessingProgramSign entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
