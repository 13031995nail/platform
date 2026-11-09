namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

public class AwpAssessingProgramBS(
    IMessageDispatcher messageDispatcher,
    AwpAssessingProgramAccess access)
: MessageBasedEntityBS<AwpAssessingProgram>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateAwpAssessingProgram(
        AwpAssessingProgram UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        AwpAssessingProgram entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
