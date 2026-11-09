namespace Tmobis.Integrations.Email.Consumer.Consumers;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;

/// <summary>
/// Потребитель запросов на массовую отправку по почте.
/// </summary>
/// <param name="messageDispatcher">Диспетчер сообщений.</param>
/// <param name="logger">Логгер.</param>
public class SendEmailMassConsumer(
  IMessageDispatcher messageDispatcher,
  ILogger<SendEmailMassConsumer> logger)
  : IMessageConsumer<SendEmailList, SendEmailResult>
{
  /// <inheritdoc/>
  public async Task<SendEmailResult> ConsumeWithResult(SendEmailList messages)
  {
    try
    {
      var results = new List<SendEmailResult>();
      foreach (var message in messages.EmailRequests)
      {
        var result = await messageDispatcher.SendWithResult<SendEmail, SendEmailResult>(message);
        results.Add(result);
      }

      return new SendEmailResult { Error = results.Any(r => r.Error) };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, @"Внутренняя ошибка обработки");

      return new SendEmailResult { Error = true };
    }
  }
}
