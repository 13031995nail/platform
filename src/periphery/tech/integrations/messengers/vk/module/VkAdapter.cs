namespace Tmobis.Integrations.Messengers.Vk;

using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class VkAdapter(
    IHttpClientFactory httpClientFactory,
    IOptions<VkOptions> optionsAccessor,
    ILogger<VkAdapter> logger)
    : IVkAdapter, IDisposable
{
    private readonly HttpClient httpClient
        = httpClientFactory.CreateClient(VkOptions.HttpClientName);
    private readonly VkOptions options = optionsAccessor.Value;

    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SendMessage(
        long peerId,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.AccessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiVersion);

        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
            "VK",
            "Outgoing",
            "SendMessage",
            peerId,
            "Started");

        try
        {
            using var requestContent = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["access_token"] = options.AccessToken.Trim(),
                    ["v"] = options.ApiVersion.Trim(),
                    ["peer_id"] = peerId.ToString(CultureInfo.InvariantCulture),
                    ["random_id"] = RandomNumberGenerator
                        .GetInt32(1, int.MaxValue)
                        .ToString(CultureInfo.InvariantCulture),
                    ["message"] = message
                });
            using var response = await httpClient.PostAsync(
                "method/messages.send",
                requestContent,
                cancellationToken);
            var responseBody = await response.Content
                .ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"VK API вернул HTTP {(int)response.StatusCode}",
                    null,
                    response.StatusCode);
            }

            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty(
                "error",
                out var error))
            {
                var errorCode = error.TryGetProperty(
                    "error_code",
                    out var code)
                    ? code.GetInt32()
                    : 0;
                var errorMessage = error.TryGetProperty(
                    "error_msg",
                    out var apiMessage)
                    ? apiMessage.GetString()
                    : null;
                throw new InvalidOperationException(
                    $"VK API вернул ошибку {errorCode}: {errorMessage}");
            }

            if (!document.RootElement.TryGetProperty("response", out _))
            {
                throw new InvalidOperationException(
                    "VK API вернул некорректный ответ");
            }

            logger.LogInformation(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "VK",
                "Outgoing",
                "SendMessage",
                peerId,
                "Succeeded");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "VK",
                "Outgoing",
                "SendMessage",
                peerId,
                "Canceled");
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "VK",
                "Outgoing",
                "SendMessage",
                peerId,
                "Failed");
            throw;
        }
    }
}
