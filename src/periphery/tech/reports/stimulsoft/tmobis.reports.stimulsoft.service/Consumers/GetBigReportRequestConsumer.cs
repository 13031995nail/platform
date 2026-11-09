namespace Tmobis.Reports.Stimulsoft.Service.Consumers;

using System.Threading.Tasks;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Extensions;
using Tmobis.Core.Medium.Producers;
using Tmobis.Files.Uploader;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Service.UseCases;

/// <summary>
/// Потребиль запросов на формирование больших отчетов.
/// </summary>
/// <param name="reportUseCase">Метод формирования отчета.</param>
/// <param name="messageDispatcher">Диспетчер сообщений.</param>
/// <param name="localFileUploader">Адаптер загрузки файлов.</param>
/// <param name="logger">Логгер.</param>
public class GetBigReportRequestConsumer(
    IReportExporter reportBuilder,
    IFileUploader localFileUploader,
    ILogger<GetBigReportRequestConsumer> logger,
    IMessageDispatcher messageDispatcher)
: IMessageConsumer<GetBigReportRequest>
{
  /// <inheritdoc/>
  public async Task Consume(GetBigReportRequest message)
  {
    ArgumentNullException.ThrowIfNull(message);

    try
    {
      var report = reportBuilder.ExportReport(message);
      var fileName = string.IsNullOrEmpty(message.FileName)
          ? $"{message.TemplateName}_{DateTime.Now:ddMMyyyy_HHmmss}"
          : message.FileName;
      var fileNameFull = $"{fileName}.csv";
      var fileId = await localFileUploader.UploadFile(report, fileNameFull);

      var response = new GetMassReportResponse()
      {
        FileName = fileNameFull,
        FileId = fileId,
        Error = false,
      };
      response.CopyContext(message);
      await messageDispatcher.Send(response);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Ошибка при выгрузке ПФ");
      // await AnswerError(message);
    }
  }

  protected virtual async Task AnswerError(GetBigReportRequest message)
  {
    var response = new GetMassReportResponse()
    {
      Error = true,
    };
    response.CopyContext(message);
    await messageDispatcher.Send(response);
  }
}
