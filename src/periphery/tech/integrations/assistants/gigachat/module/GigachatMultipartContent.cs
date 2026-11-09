namespace Tmobis.Integrations.Assistants.Gigachat;
using System.Net.Http.Headers;

/// <summary>
/// TODO: Обходной multipart, совместимый с гигачатом.
/// Имеет зашитый boundary в браузерном формате, без добавления кавычек в хедер.
/// </summary>
public class GigachatMultipartContent : MultipartFormDataContent
{
    private const string formData = "form-data";

    public GigachatMultipartContent() : base(boundary)
    {
        MediaTypeHeaderValue contentType = new("multipart/" + formData);
        contentType.Parameters.Add(
            new NameValueHeaderValue(nameof(boundary), boundary));
        Headers.ContentType = contentType;
    }

    private static readonly string boundary
    = "----WebKitFormBoundarybHOml0zk6HclOdN5";
}
