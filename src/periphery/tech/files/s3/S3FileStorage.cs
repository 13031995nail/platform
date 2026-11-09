namespace Tmobis.Files.Adapter.S3;

using System.Net;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Tmobis.Files;

/// <summary>
/// Операции с объектами S3 для файлового адаптера.
/// </summary>
public sealed class S3FileStorage(
  IAmazonS3 s3Client,
  IOptions<S3Options> options)
{
  /// <summary>
  /// Нормализованные настройки S3-хранилища.
  /// </summary>
  public S3Options Options { get; }
    = S3OptionsValidator.ValidateAndNormalize(options.Value);

  /// <summary>
  /// Сформировать ключ объекта в S3.
  /// </summary>
  /// <param name="fileId">Идентификатор файла.</param>
  /// <param name="fileName">Имя файла.</param>
  /// <returns>Ключ объекта в S3.</returns>
  public string BuildObjectKey(Guid fileId, string fileName)
  => S3ObjectKeyBuilder.Build(fileId, fileName, Options.KeyPrefix);

  /// <summary>
  /// Загрузить файл в S3.
  /// </summary>
  /// <param name="objectKey">Ключ объекта в S3.</param>
  /// <param name="fileName">Имя файла.</param>
  /// <param name="fileContent">Содержимое файла.</param>
  /// <returns>Асинхронная операция.</returns>
  public async Task UploadFile(
    string objectKey,
    string fileName,
    byte[] fileContent)
  {
    using var stream = new MemoryStream(fileContent, writable: false);
    var request = new PutObjectRequest
    {
      BucketName = Options.BucketName,
      Key = objectKey,
      InputStream = stream,
      AutoCloseStream = false,
      AutoResetStreamPosition = false,
      ContentType = MediaTypeNames.Application.Octet,
      UseChunkEncoding = Options.UseChunkEncoding
    };
    request.Metadata["original-file-name"] = fileName;

    await s3Client.PutObjectAsync(request);
  }

  /// <summary>
  /// Скачать файл из S3 и преобразовать отсутствие объекта или бакета в TmobisFileNotFoundException.
  /// </summary>
  /// <param name="fileId">Идентификатор файла для диагностики ошибок.</param>
  /// <param name="objectKey">Ключ объекта в S3.</param>
  /// <returns>Содержимое файла.</returns>
  public async Task<byte[]> DownloadFile(Guid fileId, string objectKey)
  {
    var request = new GetObjectRequest
    {
      BucketName = Options.BucketName,
      Key = objectKey
    };

    try
    {
      using var response = await s3Client.GetObjectAsync(request);
      using var memory = new MemoryStream();
      await response.ResponseStream.CopyToAsync(memory);
      return memory.ToArray();
    }
    catch (AmazonS3Exception ex)
      when (ex.StatusCode == HttpStatusCode.NotFound
        || string.Equals(ex.ErrorCode, "NoSuchKey",
          StringComparison.OrdinalIgnoreCase)
        || string.Equals(ex.ErrorCode, "NoSuchBucket",
          StringComparison.OrdinalIgnoreCase))
    {
      throw new TmobisFileNotFoundException(
        $"Файл {fileId} не найден в S3 хранилище.",
        ex);
    }
  }

  /// <summary>
  /// Сформировать временную ссылку на файл.
  /// </summary>
  /// <param name="objectKey">Ключ объекта в S3.</param>
  /// <returns>Временная ссылка.</returns>
  public Task<string> GetFileUrl(string objectKey)
  {
    var request = new GetPreSignedUrlRequest
    {
      BucketName = Options.BucketName,
      Key = objectKey,
      Verb = HttpVerb.GET,
      Expires = DateTime.UtcNow.AddMinutes(
        Options.PresignedUrlLifetimeMinutes)
    };

    return s3Client.GetPreSignedURLAsync(request);
  }

  /// <summary>
  /// Удалить объект из S3.
  /// </summary>
  /// <param name="objectKey">Ключ объекта в S3.</param>
  /// <returns>Асинхронная операция.</returns>
  public Task DeleteFile(string objectKey)
  => s3Client.DeleteObjectAsync(new DeleteObjectRequest
  {
    BucketName = Options.BucketName,
    Key = objectKey
  });
}
