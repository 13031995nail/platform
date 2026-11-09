namespace Tmobis.Files.Uploader.Local.Providers;

using Microsoft.Extensions.Options;
using System;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;
using Tmobis.Files.Uploader.Local.Options;

/// <inheritdoc/>
public class LocalFileUploader(
    IRepositoryFactory repositoryFactory,
    IOptions<FileUploadOptions> options
) : IFileUploader
{
    private readonly FileUploadOptions options = options.Value;

    /// <inheritdoc/>
    public async Task<Guid> UploadFile(byte[] fileContent, string fileName)
    {
        // TODO: Принципиально решить, как быть с такими комитами.
        // Файл сразу сохраняется во вложенном скоупе,
        // чтобы можно было отправить сообщение с закомиченным файлом.
        using var scope = repositoryFactory.MakeRepository();
        
        var file = scope.Repository.MakeEntity<IFile>();
        var directoryPath = CreateFileUploadDirectory(file.Id);
        var filePath = Path.Combine(directoryPath, fileName);
        await System.IO.File.WriteAllBytesAsync(filePath, fileContent);

        file.Name = fileName;
        file.CreateTime = DateTime.UtcNow;
        file.Size = (ulong)fileContent.LongLength;
        scope.Repository.Create(file);
        await scope.UnitOfWork.Commit();

        return file.Id;
    }

    /// <summary>
    /// Создание папки для загружаемого файла.
    /// </summary>
    /// <param name="fileId"> Идентификатор файла.</param>
    /// <returns> Путь к папке.</returns>
    protected virtual string CreateFileUploadDirectory(Guid fileId)
    {
        var directoryPath = Path.Combine(options.UploadPath, fileId.ToString());

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return directoryPath;
    }
}
