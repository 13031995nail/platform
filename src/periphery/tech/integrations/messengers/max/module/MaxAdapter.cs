namespace Tmobis.Integrations.Messengers.Max;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class MaxAdapter(
    IHttpClientFactory httpClientFactory,
    IOptions<MaxOptions> options,
    ILogger<MaxAdapter> logger)
    : IMaxAdapter, IDisposable
{
    private static readonly JsonSerializerOptions jsonOptions
        = new(JsonSerializerDefaults.Web);

    private readonly HttpClient httpClient
        = httpClientFactory.CreateClient(MaxOptions.HttpClientName);
    private readonly MaxOptions options = options.Value;

    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SendMessage(
        long chatId,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        await ExecuteOutgoingRequest(
            "SendMessage",
            chatId,
            () => SendMessageBody(
                chatId,
                new { text = message },
                cancellationToken),
            cancellationToken);
    }

    public async Task RequestContact(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var body = new
        {
            text = "Для регистрации нажмите «Поделиться контактом».",
            attachments = new[]
            {
                new
                {
                    type = "inline_keyboard",
                    payload = new
                    {
                        buttons = new[]
                        {
                            new[]
                            {
                                new
                                {
                                    type = "request_contact",
                                    text = "Поделиться контактом"
                                }
                            }
                        }
                    }
                }
            }
        };

        await ExecuteOutgoingRequest(
            "RequestContact",
            chatId,
            () => SendMessageBody(chatId, body, cancellationToken),
            cancellationToken);
    }

    public async Task RegisterWebhook(
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.WebhookUrl);

        var body = new MaxSubscriptionRequest
        {
            Url = options.WebhookUrl,
            UpdateTypes = ["message_created"],
            Secret = string.IsNullOrWhiteSpace(options.WebhookSecret)
                ? null
                : options.WebhookSecret
        };

        await ExecuteOutgoingRequest(
            "RegisterWebhook",
            null,
            async () =>
            {
                var result = await PostAsJson<
                    MaxSubscriptionRequest,
                    MaxSubscriptionResult>(
                    "subscriptions",
                    body,
                    cancellationToken);
                if (!result.Success)
                {
                    throw new InvalidOperationException(
                        $"MAX не зарегистрировал webhook: {result.Message}");
                }
            },
            cancellationToken);
    }

    protected virtual async Task SendMessageBody<TBody>(
        long chatId,
        TBody body,
        CancellationToken cancellationToken)
    {
        var path = $"messages?chat_id={chatId}";
        await PostAsJson(path, body, cancellationToken);
    }

    protected virtual async Task PostAsJson<TBody>(
        string path,
        TBody body,
        CancellationToken cancellationToken)
    {
        ValidateAccessToken();
        using var response = await httpClient.PostAsJsonAsync(
            path,
            body,
            jsonOptions,
            cancellationToken);
        await EnsureSuccess(response, cancellationToken);
    }

    protected virtual async Task<TResult> PostAsJson<TBody, TResult>(
        string path,
        TBody body,
        CancellationToken cancellationToken)
    {
        ValidateAccessToken();
        using var response = await httpClient.PostAsJsonAsync(
            path,
            body,
            jsonOptions,
            cancellationToken);
        await EnsureSuccess(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResult>(
            jsonOptions,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "MAX API вернул пустой ответ");
    }

    private void ValidateAccessToken()
        => ArgumentException.ThrowIfNullOrWhiteSpace(options.AccessToken);

    private async Task ExecuteOutgoingRequest(
        string operation,
        long? messengerId,
        Func<Task> request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
            "MAX",
            "Outgoing",
            operation,
            messengerId,
            "Started");

        try
        {
            await request();
            logger.LogInformation(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "MAX",
                "Outgoing",
                operation,
                messengerId,
                "Succeeded");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "MAX",
                "Outgoing",
                operation,
                messengerId,
                "Canceled");
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "MAX",
                "Outgoing",
                operation,
                messengerId,
                "Failed");
            throw;
        }
    }

    private static async Task EnsureSuccess(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content
            .ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            $"MAX API вернул {(int)response.StatusCode}: {responseBody}",
            null,
            response.StatusCode);
    }

    private sealed record MaxSubscriptionRequest
    {
        [JsonPropertyName("url")]
        public required string Url { get; init; }

        [JsonPropertyName("update_types")]
        public required string[] UpdateTypes { get; init; }

        [JsonPropertyName("secret")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Secret { get; init; }
    }

    private sealed record MaxSubscriptionResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; }
    }
}
