namespace Tmobis.Integrations.Messengers.Mattermost;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Integrations.Messengers.Mattermost.Models;

public sealed class MattermostAdapter(
    IHttpClientFactory httpClientFactory,
    IOptions<MattermostOptions> optionsAccessor,
    ILogger<MattermostAdapter> logger)
    : IMattermostAdapter, IDisposable
{
    private readonly HttpClient httpClient
        = httpClientFactory.CreateClient(MattermostOptions.HttpClientName);
    private readonly MattermostOptions options = optionsAccessor.Value;

    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SendMessage(
        string userId,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ValidateOptions();

        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
            "Mattermost",
            "Outgoing",
            "SendMessage",
            userId,
            "Started");

        try
        {
            using var channelResponse = await httpClient.PostAsJsonAsync(
                "api/v4/channels/direct",
                new[] { options.BotUserId, userId },
                cancellationToken);
            var channel = await ReadResponse<MattermostChannel>(
                channelResponse,
                cancellationToken);
            if (string.IsNullOrWhiteSpace(channel.Id))
            {
                throw new InvalidOperationException(
                    "Mattermost API не вернул идентификатор личного канала");
            }

            using var postResponse = await httpClient.PostAsJsonAsync(
                "api/v4/posts",
                new MattermostCreatePost
                {
                    ChannelId = channel.Id,
                    Message = message
                },
                cancellationToken);
            await EnsureSuccess(postResponse, cancellationToken);

            logger.LogInformation(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "Mattermost",
                "Outgoing",
                "SendMessage",
                userId,
                "Succeeded");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "Mattermost",
                "Outgoing",
                "SendMessage",
                userId,
                "Canceled");
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "Mattermost",
                "Outgoing",
                "SendMessage",
                userId,
                "Failed");
            throw;
        }
    }

    private void ValidateOptions()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ServerUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.AccessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.BotUserId);
    }

    private static async Task<T> ReadResponse<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        await EnsureSuccess(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException(
                "Mattermost API вернул пустой ответ");
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
            $"Mattermost API вернул HTTP {(int)response.StatusCode}: {responseBody}",
            null,
            response.StatusCode);
    }
}
