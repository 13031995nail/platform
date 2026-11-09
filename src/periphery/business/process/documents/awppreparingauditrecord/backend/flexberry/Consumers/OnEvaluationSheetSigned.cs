namespace Tmobis.Business.Process.Documents;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnEvaluationSheetSigned(
    ILogger<OnEvaluationSheetSigned> logger,
    IRepository repository,
    IAwpPreparingAuditRecordProvider awpPreparingAuditRecordProvider)
  : IMessageConsumer<EvaluationSheetSigned, IEntityUpdatedResult>
{
    public async Task<IEntityUpdatedResult> ConsumeWithResult(EvaluationSheetSigned message)
    {
        List<IEntity> toUpdateReturn = new List<IEntity>();
        try
        {
            // при подписании акта оценки готовности сохраняем его в ЖК и проставляем дату в поле ReadinessActDate
            ArgumentNullException.ThrowIfNull(message);
            var act = message.Act;
            var record = await repository.FirstOrDefault<AwpPreparingAuditRecord>(x => x.Awp.Id == act.Awp.Id
                && x.WorkObject.Id == act.WorkObject.Id);
            if (record != null)
            {
                record.EvaluationSheet = act;
                record.EvaluationSheetDate = act.ChangeStatusDate;
                record.IndexReadiness = act.ObjectReadinessIndex;
                var reqs = await repository.Find<EvaluationSheetRequirement>(x => x.EvaluationSheet.Id == act.Id);
                record.HasEvaluationSheetProblem = reqs.Any(x => !string.IsNullOrEmpty(x.ProblemComment));
                toUpdateReturn.Add(record);
                repository.Update(record);
                await awpPreparingAuditRecordProvider.UpdatePartnerRecords(record.Awp.Id, record.WorkObject.Organization.Id);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении журнала контроля при подписании оценочного листа");
            return new EntityUpdatedResult() { Continue = false };
        }
        return new EntityUpdatedResult
        {
            Continue = true,
            EntitiesToUpdate = toUpdateReturn
        };
    }
}
