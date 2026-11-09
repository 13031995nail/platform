namespace Tmobis.Integrations.Email.Consumer.Consumers;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Files.Provider;

/// <summary>
/// Потребитель запросов на отправку по почте.
/// </summary>
/// <param name="remoteFileProvider">Пакет обработки.</param>
/// <param name="mailKitProducer">Диспетчер mailkit.</param>
/// <param name="logger">Логгер.</param>
public class SendEmailConsumer(
  IFileProvider remoteFileProvider,
  IMessageDispatcher messageDispatcher,
  ILogger<SendEmailConsumer> logger)
  : IMessageConsumer<SendEmail, SendEmailResult>
{
  /// <inheritdoc/>
  public async Task<SendEmailResult> ConsumeWithResult(SendEmail message)
  {
    try
    {
      var filesAttach = new Dictionary<string, byte[]>();
      if (message.FileIds != null && message.FileIds.Count > 0)
      {
        foreach (var File in message.FileIds)
        {
          byte[] image = await remoteFileProvider.GetFile(File.FileId, File.FileName);
          filesAttach.Add(File.FileName, image);
        }
      }

      SendEmailPrepared prepared = new SendEmailPrepared
      {
        Addresses = message.Addresses,
        Subject = message.Subject,
        Body = message.Body,
        HighPriority = message.HighPriority,
        DiffLetters = message.DiffLetters,
        Files = filesAttach
      };

      var result = await messageDispatcher.SendWithResult<SendEmailPrepared,SendEmailResult>(prepared);
      return result;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, @"Внутренняя ошибка обработки");
      return new SendEmailResult { Error = true };
    }
  }
}
