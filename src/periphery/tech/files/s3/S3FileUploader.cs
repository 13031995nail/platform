namespace Tmobis.Files.Adapter.S3;

using Microsoft.Extensions.Logging;
using Tmobis.Business.Core.Models.Files;
using Core.Repositories;
using Uploader;

/// <summary>
/// Адаптер загрузки файлов в S3.
/// </summary>
public sealed class S3FileUploader(
    IRepositoryFactory repositoryFactory,
    S3FileStorage storage,
    ILogger<S3FileUploader> logger)
    : IFileUploader
{
    /// <inheritdoc/>
    public async Task<Guid> UploadFile(byte[] fileContent, string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileContent);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        // TODO: Принципиально решить, как быть с такими комитами.
        // Файл сразу сохраняется во вложенном скоупе,
        // чтобы можно было отправить сообщение с закомиченным файлом.
        using var scope = repositoryFactory.MakeRepository();
        
        var file = scope.Repository.MakeEntity<IFile>();

        // Экранируем кириллицу 
        var safeFileNameForStorage = Uri.EscapeDataString(fileName);

        var objectKey = storage.BuildObjectKey(file.Id, safeFileNameForStorage);
        await storage.UploadFile(objectKey, safeFileNameForStorage, fileContent);

        try
        {
            file.Name = fileName;
            file.CreateTime = DateTime.UtcNow;
            file.Size = (ulong)fileContent.LongLength;
            scope.Repository.Create(file);
            await scope.UnitOfWork.Commit();
        }
        catch
        {
            await DeleteUploadedObject(file.Id, objectKey);
            throw;
        }

        return file.Id;
    }

    private async Task DeleteUploadedObject(Guid fileId, string objectKey)
    {
        try
        {
            await storage.DeleteFile(objectKey);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Не удалось удалить объект S3 для файла {FileId} после ошибки сохранения метаданных.",
                fileId);
        }
    }
}
