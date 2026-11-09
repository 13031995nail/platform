namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

[Obsolete("БС не используется, так как генерируется много объектов для сохранения", true)]
public class EvaluationScheduleBS(IMessageDispatcher messageDispatcher)
: MessageBasedEntityBS<AwpAssessingProgramEvaluationSchedule>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateAwpAssessingProgramEvaluationSchedule(
        AwpAssessingProgramEvaluationSchedule UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];
}
