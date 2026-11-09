namespace Tmobis.Files.PrimaryAdapter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tmobis.Medium.WebApi;

/// <summary>
/// Построение endpoint-ов файлового API.
/// </summary>
internal sealed class FilesEndpointBuilder : ApiEndpointBuilder
{
    /// <summary>
    /// Добавление маршрутов загрузки и получения файлов в WebApi pipeline.
    /// </summary>
    public override void Build(IEndpointRouteBuilder endpoints)
    {
        var fileGroup = endpoints.MapGroup(BuildEndpoint("File"))
            .WithTags("File")
            .RequireAuthorization();

        fileGroup.MapGet("{id:guid}", (
            Guid id,
            [FromServices] FilesEndpointHandler handler)
            => handler.DownloadFile(id));

        fileGroup.MapGet("{id:guid}/metadata", (
            Guid id,
            [FromServices] FilesEndpointHandler handler)
            => handler.GetFileMetadata(id));

        fileGroup.MapPut("", (
            HttpRequest request,
            [FromQuery] string? fileName,
            [FromServices] FilesEndpointHandler handler)
            => handler.UploadFile(request, fileName));
    }
}
