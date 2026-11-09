namespace Tmobis.Business.Process.Documents;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAwpReadinessPassportSigned(
    ILogger<OnAwpReadinessPassportSigned> logger,
    IRepository repository)
  : IMessageConsumer<AwpReadinessPassportSigned, IEntityUpdatedResult>
{
    public async Task<IEntityUpdatedResult> ConsumeWithResult(AwpReadinessPassportSigned message)
    {
        List<IEntity> toUpdateReturn = new List<IEntity>();
        try
        {
            // при подписании паспорта готовности сохраняем его в ЖК и проставляем дату в поле PassportDate
            ArgumentNullException.ThrowIfNull(message);
            var act = message.Act;
            var objects = await repository.Find<AwpReadinessPassportObject>(x => x.Act.Id == act.Id);
            var objectIds = objects.Select(x => x.WorkObject.Id).Distinct().Cast<object>().ToList();
            var records = objectIds.Count == 0
                ? []
                : await repository.Find<AwpPreparingAuditRecord>(x => x.Awp.Id == act.Awp.Id
                    && x.WorkObject.Organization.Id == act.Organization.Id
                    && objectIds.Contains(x.WorkObject.Id));
            foreach (var record in records)
            {
                record.AwpReadinessPassport = act;
                record.PassportDate = act.ChangeStatusDate;
                repository.Update(record);
                toUpdateReturn.Add(record);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении журнала контроля при подписании паспорта готовности");
            return new EntityUpdatedResult() { Continue = false };
        }
        return new EntityUpdatedResult
        {
            Continue = true,
            EntitiesToUpdate = toUpdateReturn
        };
    }
}
