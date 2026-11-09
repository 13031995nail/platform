namespace Tmobis.Files.Provider.Remote;

using System;
using System.Net;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;
using Tmobis.Files;

/// <summary>
/// Провайдер получения файлов из внешнего файлового сервиса.
/// </summary>
public class RemoteFileProvider(
    HttpClient httpClient,
    IRepository repository,
    IOptions<RemoteFileProviderOptions> options)
: IFileProvider
{
    private readonly RemoteFileProviderOptions options = options.Value;

    /// <summary>
    /// Получение имени файла из записи метаданных и содержимого из внешнего файлового сервиса.
    /// </summary>
    public virtual async Task<byte[]> GetFile(Guid fileId)
    {
        var file = await repository.GetById<IFile>(fileId)
            ?? throw new TmobisFileNotFoundException("Файл не найден");

        return await GetFile(fileId, file.Name);
    }

    /// <summary>
    /// Получение содержимого файла из внешнего сервиса с преобразованием HTTP 404 в TmobisFileNotFoundException.
    /// </summary>
    public async Task<byte[]> GetFile(Guid fileId, string fileName)
    {
        using var response = await httpClient.GetAsync(
            $"?{options.FileIdParam}={fileId}"
            + $"&{options.FileNameParam}={Uri.EscapeDataString(fileName)}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new TmobisFileNotFoundException("Файл не найден");
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Получение пути к файлу через удалённый провайдер сейчас не реализовано.
    /// </summary>
    public Task<byte[]> GetFilePath(Guid fileId, string fileName)
    {
        throw new NotImplementedException();
    }
}
