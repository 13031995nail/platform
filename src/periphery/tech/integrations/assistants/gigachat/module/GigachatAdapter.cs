namespace Tmobis.Integrations.Assistants.Gigachat;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Assistants.Base.Messages;

public class GigachatAdapter(
    IMemoryCache cache,
    HttpClient httpClient,
    IOptions<GigachatOptions> options)
    : IMessageProducer
{
    private readonly IMemoryCache cache = cache;
    private readonly HttpClient httpClient = httpClient;
    private readonly GigachatOptions options = options.Value;
    private readonly string tokenCacheName = "gigachatToken";

    public async Task<IMessageResult> SendWithResult(IMessage rawMessage)
    {
        if (rawMessage is not AssistantRequest message)
        {
            throw new InvalidOperationException("Неверный тип сообщения");
        }

        try
        {
            var apiRequest = new GigachatApiRequest()
            {
                Model = options.Model
            };
            var apiMessage = new GigachatApiMessage()
            {
                Content = message.Message
            };
            apiRequest.Messages.Add(apiMessage);
            string? fileId = null;
            if (message.HasFile)
            {
                fileId = await AttachFile(message);
                if (!string.IsNullOrEmpty(fileId))
                {
                    apiMessage.Attachments.Add(fileId);
                }
            }

            var result = await Send(apiRequest);

            if (!string.IsNullOrEmpty(fileId))
            {
                await DeleteFile(fileId);
            }

            return new AssistantResponse() { Error = false, Message = result };
        }
        catch (Exception ex)
        {
            return new AssistantResponse() { Error = true, Message = ex.Message };
        }
    }

    protected virtual async Task<string?> Send(GigachatApiRequest apiRequest)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            options.RequestUrl);

        var token = await GetToken();
        request.Headers.Add("Authorization", $"Bearer {token}");
        request.Headers.Add("Accept", "application/json");
        request.Content = JsonContent.Create(apiRequest);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseMessage = await response.Content
            .ReadFromJsonAsync<GigachatApiResponse>();
        return responseMessage?.Choices?.FirstOrDefault()?.Message?.Content;
    }

    protected virtual async Task<string?> AttachFile(AssistantRequest message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.File);
        ArgumentNullException.ThrowIfNull(message.FileMime);
        ArgumentNullException.ThrowIfNull(message.FileName);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            options.FileUploadUrl);
        var token = await GetToken();
        request.Headers.Add("Authorization", $"Bearer {token}");

        using var content = new GigachatMultipartContent();
        request.Content = content;

        var fileContent = new ByteArrayContent(message.File);
        fileContent.Headers.ContentType
            = new MediaTypeHeaderValue(message.FileMime);
        content.Add(fileContent, name: "\"file\"", fileName: message.FileName);

        var purposeContent = new StringContent("general");
        purposeContent.Headers.ContentType
            = new MediaTypeHeaderValue("text/plain");
        content.Add(purposeContent, name: "\"purpose\"");
        
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var fileResponse = await response.Content
            .ReadFromJsonAsync<GigachatApiFileResponse>();
        return fileResponse?.Id;
    }

    protected virtual async Task DeleteFile(string fileId)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            options.FileDeleteUrl.Replace("{id}", fileId));
        request.Headers.Add("Accept", "application/json");
        var token = await GetToken();
        request.Headers.Add("Authorization", $"Bearer {token}");
        await httpClient.SendAsync(request);
    }

    protected virtual Task<string?> GetToken()
    => cache.GetOrCreateAsync(tokenCacheName, RequestToken);

    protected virtual async Task<string?> RequestToken(ICacheEntry entry)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            options.AuthUrl);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("RqUID", Guid.NewGuid().ToString());
        request.Headers.Add("Authorization", $"Basic {options.Token}");
        var collection = new List<KeyValuePair<string, string>>
        {
          new("scope", options.Scope)
        };
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var authResponse =
            await response.Content.ReadFromJsonAsync<GigachatApiAuthResponse>();
        entry.AbsoluteExpirationRelativeToNow = options.TokenExpiration;
        return authResponse?.AccessToken;
    }

    public Task Send(IMessage message)
    {
        throw new NotImplementedException();
    }

    public Task Publish(IMessage message)
    {
        throw new NotImplementedException();
    }
}