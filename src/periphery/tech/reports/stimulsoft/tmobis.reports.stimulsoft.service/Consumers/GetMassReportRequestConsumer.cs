namespace Tmobis.Reports.Stimulsoft.Service.Consumers;

using System.Threading.Tasks;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Service.UseCases;
using Tmobis.Core.Medium.Extensions;
using Tmobis.Files.Uploader;

/// <summary>
/// Потребитель запросов на создание массовых отчетов.
/// </summary>
/// <param name="reportUseCase">Метод формирования отчета.</param>
/// <param name="messageDispatcher">Диспетчер сообщений.</param>
/// <param name="localFileUploader">Адаптер загрузки файлов.</param>
/// <param name="logger">Логгер.</param>
public class GetMassReportRequestConsumer(
    IMassReportExporter reportBuilder,
    IFileUploader localFileUploader,
    ILogger<GetMassReportRequestConsumer> logger,
    IMessageDispatcher messageDispatcher)
  : IMessageConsumer<GetMassReportRequest>
{
  /// <inheritdoc/>
  public async Task Consume(GetMassReportRequest message)
  {
    var reportParams = message?.ReportParams;
    if (reportParams == null)
    {
      logger.LogError(@"Запрос не обработан. Неправильно переданные параметры = {Message}", message);
      await AnswerError(message);
      return;
    }

    try
    {
      const string fileName = "Acts.zip";
      var archive = await reportBuilder.ExportMassReport(message);
      var fileId = await localFileUploader.UploadFile(archive, fileName);

      var response = new GetMassReportResponse()
      {
        FileName = fileName,
        FileId = fileId,
        Error = false,
      };
      response.CopyContext(message);
      await messageDispatcher.Send(response);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, @"Внутренняя ошибка обработки");
      await AnswerError(message);
    }
  }

  protected virtual async Task AnswerError(GetMassReportRequest message)
  {
    var response = new GetMassReportResponse()
    {
      Error = true,
    };
    response.CopyContext(message);
    await messageDispatcher.Send(response);
  }
}
