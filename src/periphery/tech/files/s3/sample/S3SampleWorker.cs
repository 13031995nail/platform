namespace Tmobis.Files.Adapter.S3.Sample;

using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Files.Provider;
using Tmobis.Files.Uploader;

internal sealed class S3SampleWorker(
  IFileUploader fileUploader,
  IFileProvider fileProvider,
  IOptions<S3Options> s3Options,
  IOptions<S3SampleOptions> sampleOptions,
  ILogger<S3SampleWorker> logger,
  IHostApplicationLifetime lifetime)
  : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    try
    {
      var s3 = s3Options.Value;
      var sample = sampleOptions.Value;
      var payload = Encoding.UTF8.GetBytes(sample.Payload);

      logger.LogInformation(
        "Запуск S3 sample. Endpoint: {Endpoint}, Bucket: {Bucket}",
        s3.ServiceUrl,
        s3.BucketName);

      var fileId = await fileUploader.UploadFile(payload, sample.FileName);
      logger.LogInformation("Файл загружен в S3. Id: {FileId}", fileId);

      var downloaded = await fileProvider.GetFile(fileId, sample.FileName);
      if (!payload.SequenceEqual(downloaded))
      {
        throw new InvalidOperationException(
          "Скачанное содержимое не совпадает с исходным.");
      }

      _ = await fileProvider.GetFilePath(fileId, sample.FileName);

      logger.LogInformation(
        "Проверка S3 sample завершена успешно. Presigned URL сформирован.");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Проверка S3 sample завершилась с ошибкой.");
      throw;
    }
    finally
    {
      lifetime.StopApplication();
    }
  }
}
