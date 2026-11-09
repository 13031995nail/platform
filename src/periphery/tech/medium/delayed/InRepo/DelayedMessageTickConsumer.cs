namespace Tmobis.Medium.Delayed.InRepo;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Repositories;
using Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ICSSoft.STORMNET.KeyGen;
using Microsoft.Extensions.Logging;

public class DelayedMessageTickConsumer(
    IRepository repository,
    IUnitOfWork unitOfWork,
    IMessageDispatcher dispatcher,
    ILogger<DelayedMessageTickConsumer> logger
): IMessageConsumer<DelayedMessageTick>
{

  public async Task Consume(DelayedMessageTick message) 
  {
    var now = DateTime.UtcNow;
    try
    {
      var dueEntites = await repository
          .Find<DelayedMessageEntity>(entity => entity.Time <= now);

      foreach (var entity in dueEntites)
      {
        try
        {
          var delayedMessage = MapEntityToMessage(entity);
          var delayedMessageNow = new DelayedMessageNow { Wrappee = delayedMessage };

          await dispatcher.Send(delayedMessageNow);
          repository.Delete(entity);
          await unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке отложенного сообщения");
        }
      }
    }
    catch (Exception ex)
    {
        logger.LogError(ex,"Ошибка при обработке отложенного сообщения");
    }
  }

  private DelayedMessage MapEntityToMessage(IDelayedMessageEntity entity)
  {
    try
    {
      var settings = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
      };

      var message = JsonConvert.DeserializeObject<IMessage>(entity.RawMessage, settings);

      if (message == null)
      {
        return null;
      }

      return new DelayedMessage
      {
        Wrappee = message,
        Time = entity.Time,
      };
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при обработке отложенного сообщения");
      return null;
    }
  }
}
