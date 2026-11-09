namespace Tmobis.Files.PrimaryAdapter;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;
using Tmobis.Files;
using Tmobis.Files.Provider;
using Tmobis.Files.Uploader;

/// <summary>
/// Обработка HTTP-запросов загрузки и получения файлов.
/// </summary>
internal sealed class FilesEndpointHandler(
    IRepository repository,
    IUnitOfWork unitOfWork,
    IFileUploader uploader,
    IFileProvider provider,
    IOptions<FilesPrimaryAdapterOptions> optionsAccessor,
    FileUploadRequestValidator validator,
    RequestBodyReader bodyReader)
{
    private readonly FilesPrimaryAdapterOptions options = optionsAccessor.Value;

    /// <summary>
    /// Приём содержимого файла из тела запроса, загрузка через файловый adapter и фиксация изменений репозитория.
    /// </summary>
    public async Task<IResult> UploadFile(
        HttpRequest request,
        string? fileName)
    {
        if (!validator.IsFileNameValid(fileName))
        {
            return Results.BadRequest();
        }

        if (!validator.IsExtensionAllowed(Path.GetExtension(fileName!), options.AllowedExtensions))
        {
            return Results.BadRequest();
        }

        if (!validator.IsMimeTypeAllowed(request.ContentType, options.AllowedMimeTypes))
        {
            return Results.BadRequest();
        }

        var maxUploadSize = options.MaxUploadSizeBytes;
        if (maxUploadSize > 0 && request.ContentLength > maxUploadSize)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        var body = await bodyReader.ReadBody(request, maxUploadSize);
        if (body.IsTooLarge)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        if (body.Content.Length == 0)
        {
            return Results.BadRequest();
        }

        var fileId = await uploader.UploadFile(body.Content, fileName!);
        await unitOfWork.Commit();

        return Results.Created($"/api/File/{fileId}", fileId);
    }

    /// <summary>
    /// Получение содержимого файла после проверки записи метаданных в репозитории.
    /// </summary>
    public async Task<IResult> DownloadFile(Guid id)
    {
        var file = await repository.GetById<IFile>(id);
        if (file == null)
        {
            return Results.NotFound();
        }

        try
        {
            var content = await provider.GetFile(id, file.Name);
            return Results.File(content, "application/octet-stream", file.Name);
        }
        catch (TmobisFileNotFoundException)
        {
            return Results.NotFound();
        }
    }

    /// <summary>
    /// Получение метаданных файла через GetFilePath из provider.
    /// Возвращает { url, name, mimeType } в теле ответа.
    /// </summary>
    public async Task<IResult> GetFileMetadata(Guid id)
    {
        var file = await repository.GetById<IFile>(id);
        if (file == null)
        {
            return Results.NotFound();
        }

        try
        {
            // GetFilePath возвращает UTF-8 закодированный URL
            var urlBytes = await provider.GetFilePath(id, file.Name);
            var url = System.Text.Encoding.UTF8.GetString(urlBytes);

            // Определяем MIME type по расширению файла
            var mimeType = GetContentTypeByExtension(file.Name);

            return Results.Ok(new { url, name = file.Name, mimeType });
        }
        catch (TmobisFileNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private string GetContentTypeByExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            // Документы
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".rtf" => "application/rtf",
            ".txt" => "text/plain",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".odt" => "application/vnd.oasis.opendocument.text",
            ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".csv" => "text/csv",

            // Изображения
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".tiff" => "image/tiff",
            ".tif" => "image/tiff",

            // Видео
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".ogg" => "video/ogg",
            ".mov" => "video/quicktime",
            ".avi" => "video/x-msvideo",
            ".mkv" => "video/x-matroska",

            // Аудио
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".aac" => "audio/aac",
            ".flac" => "audio/flac",
            ".m4a" => "audio/mp4",

            // Архивы
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".tar" => "application/x-tar",
            ".gz" => "application/gzip",

            // Веб и код
            ".html" => "text/html",
            ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "text/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",

            // Шрифты
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/otf",
            ".otf" => "font/otf",

            // Дефолтное значение
            _ => "application/octet-stream"
        };
    }
}
