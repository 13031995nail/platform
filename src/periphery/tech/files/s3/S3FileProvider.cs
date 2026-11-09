namespace Tmobis.Files.Adapter.S3;

using System.Text;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;
using Tmobis.Files;
using Tmobis.Files.Provider;

/// <summary>
/// Адаптер получения файлов из S3.
/// </summary>
public sealed class S3FileProvider(
  IRepository repository,
  S3FileStorage storage)
  : IFileProvider
{
  /// <inheritdoc/>
  public async Task<byte[]> GetFile(Guid fileId)
  {
    var file = await GetExistingFile(fileId);
    return await storage.DownloadFile(
      fileId,
      storage.BuildObjectKey(fileId, file.Name));
  }

  /// <inheritdoc/>
  public async Task<byte[]> GetFile(Guid fileId, string fileName)
  {
    var objectKey = await GetExistingFileObjectKey(fileId, fileName);

    return await storage.DownloadFile(fileId, objectKey);
  }

  /// <inheritdoc/>
  public async Task<byte[]> GetFilePath(Guid fileId, string fileName)
  {
    var objectKey = await GetExistingFileObjectKey(fileId, fileName);
    var url = await storage.GetFileUrl(objectKey);
    return Encoding.UTF8.GetBytes(url);
  }

  /// <summary>
  /// Проверяет запись метаданных File и возвращает ключ S3 только при совпадении имени файла.
  /// </summary>
  private async Task<string> GetExistingFileObjectKey(
    Guid fileId,
    string fileName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

    var file = await GetExistingFile(fileId);
    if (!string.Equals(file.Name, fileName, StringComparison.Ordinal))
    {
      throw new TmobisFileNotFoundException(
        $"Файл {fileId} не найден в репозитории.");
    }
    // Экранируем кириллицу 
    var safeFileNameForStorage = Uri.EscapeDataString(file.Name);
    return BuildObjectKey(fileId, safeFileNameForStorage);
  }

  /// <summary>
  /// Получает запись метаданных File или сообщает, что файл не найден.
  /// </summary>
  private async Task<IFile> GetExistingFile(Guid fileId)
  => await repository.GetById<IFile>(fileId)
    ?? throw new TmobisFileNotFoundException(
      $"Файл {fileId} не найден в репозитории.");

  /// <summary>
  /// Формирует ключ объекта S3 по идентификатору и имени файла.
  /// </summary>
  private string BuildObjectKey(Guid fileId, string fileName)
  => storage.BuildObjectKey(fileId, fileName);
}
