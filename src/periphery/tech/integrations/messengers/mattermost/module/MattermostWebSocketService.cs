namespace Tmobis.Integrations.Messengers.Mattermost;

using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Mattermost.Messages;
using Tmobis.Integrations.Messengers.Mattermost.Models;
using Websocket.Client;

internal sealed class MattermostWebSocketService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<MattermostOptions> optionsAccessor,
    ILogger<MattermostWebSocketService> logger)
    : BackgroundService
{
    private readonly MattermostOptions options = optionsAccessor.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (!options.ListenForMessages)
        {
            logger.LogInformation(
                "Получение сообщений Mattermost отключено");
            return;
        }

        if (string.IsNullOrWhiteSpace(options.ServerUrl)
            || string.IsNullOrWhiteSpace(options.AccessToken)
            || string.IsNullOrWhiteSpace(options.BotUserId))
        {
            logger.LogWarning(
                "Получение сообщений Mattermost пропущено: настройки заполнены не полностью");
            return;
        }

        var messages = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions { SingleReader = true });
        using var client = new WebsocketClient(BuildWebSocketUri())
        {
            ReconnectTimeout = null,
            ErrorReconnectTimeout = options.ReconnectDelay
        };
        using var messageSubscription = client.MessageReceived.Subscribe(
            message =>
            {
                if (!string.IsNullOrWhiteSpace(message.Text))
                {
                    messages.Writer.TryWrite(message.Text);
                }
            });
        using var reconnectionSubscription = client.ReconnectionHappened
            .Subscribe(_ =>
            {
                Authenticate(client);
                logger.LogInformation(
                    "Соединение Mattermost WebSocket установлено");
            });
        using var disconnectionSubscription = client.DisconnectionHappened
            .Subscribe(disconnection => logger.LogWarning(
                disconnection.Exception,
                "Соединение Mattermost WebSocket прервано: {Reason}",
                disconnection.Type));

        await client.Start();
        await foreach (var json in messages.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await HandleMessage(json);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Не удалось обработать событие Mattermost WebSocket");
            }
        }
    }

    private async Task HandleMessage(string json)
    {
        var message = JsonSerializer.Deserialize<MattermostWebSocketMessage>(
            json);
        if (message?.SequenceReply == 1
            && !string.Equals(message.Status, "OK", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Mattermost отклонил авторизацию WebSocket");
        }

        var data = message?.Data;
        if (!string.Equals(message?.Event, "posted", StringComparison.Ordinal)
            || data == null
            || !string.Equals(
                data.ChannelType,
                "D",
                StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(data.Post))
        {
            return;
        }

        var post = JsonSerializer.Deserialize<MattermostPost>(data.Post);
        if (post == null
            || string.IsNullOrWhiteSpace(post.Id)
            || string.IsNullOrWhiteSpace(post.UserId)
            || string.Equals(
                post.UserId,
                options.BotUserId,
                StringComparison.Ordinal))
        {
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var messageDispatcher = scope.ServiceProvider
            .GetRequiredService<IMessageDispatcher>();
        await messageDispatcher.Send(new MattermostPostReceived
        {
            PostId = post.Id,
            UserId = post.UserId,
            ChannelId = post.ChannelId,
            Text = post.Message
        });
    }

    private void Authenticate(WebsocketClient client)
        => client.Send(JsonSerializer.Serialize(new
        {
            seq = 1,
            action = "authentication_challenge",
            data = new { token = options.AccessToken }
        }));

    private Uri BuildWebSocketUri()
    {
        var serverUri = new Uri(options.ServerUrl.TrimEnd('/') + "/");
        var builder = new UriBuilder(serverUri)
        {
            Scheme = serverUri.Scheme switch
            {
                "http" => "ws",
                "https" => "wss",
                _ => throw new InvalidOperationException(
                    "Mattermost:ServerUrl должен использовать HTTP или HTTPS")
            },
            Path = serverUri.AbsolutePath.TrimEnd('/') + "/api/v4/websocket"
        };
        return builder.Uri;
    }
}
