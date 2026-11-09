namespace Tmobis.Files.PrimaryAdapter;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Чтение тела HTTP-запроса с контролем ограничения размера.
/// Чтение прерывается при достижении настроенного лимита.
/// </summary>
internal sealed class RequestBodyReader
{
    /// <summary>
    /// Получение содержимого тела запроса и признака превышения настроенного лимита.
    /// При превышении лимита чтение прерывается без записи очередного блока.
    /// </summary>
    public async Task<(byte[] Content, bool IsTooLarge)> ReadBody(
        HttpRequest request,
        long? maxUploadSize)
    {
        using var memoryStream = new MemoryStream();
        var buffer = new byte[81920];
        int bytesRead;

        while ((bytesRead = await request.Body.ReadAsync(buffer)) > 0)
        {
            if (maxUploadSize > 0 && memoryStream.Length + bytesRead > maxUploadSize)
            {
                return ([], true);
            }

            memoryStream.Write(buffer, 0, bytesRead);
        }

        return (memoryStream.ToArray(), false);
    }
}
