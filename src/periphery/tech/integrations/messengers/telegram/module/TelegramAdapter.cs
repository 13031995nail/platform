namespace Tmobis.Integrations.Messengers.Telegram;

using global::Telegram.Bot;
using global::Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class TelegramAdapter(
    ITelegramBotClient botClient,
    IOptions<TelegramOptions> optionsAccessor,
    ILogger<TelegramAdapter> logger)
    : ITelegramAdapter
{
    private readonly TelegramOptions options = optionsAccessor.Value;

    public async Task SendMessage(
        long chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        await ExecuteOutgoingRequest(
            "SendMessage",
            chatId,
            async () => await botClient.SendMessage(
                chatId,
                text,
                cancellationToken: cancellationToken),
            cancellationToken);
    }

    public async Task RequestContact(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var button = new KeyboardButton("Поделиться контактом")
        {
            RequestContact = true
        };
        var keyboard = new ReplyKeyboardMarkup([[button]])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        await ExecuteOutgoingRequest(
            "RequestContact",
            chatId,
            async () => await botClient.SendMessage(
                chatId,
                "Для регистрации нажмите «Поделиться контактом».",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken),
            cancellationToken);
    }

    public async Task RegisterWebhook(
        CancellationToken cancellationToken = default)
    {
        if (options.WebhookUrl == null)
        {
            throw new InvalidOperationException(
                "Не задан Telegram:WebhookUrl");
        }

        await ExecuteOutgoingRequest(
            "RegisterWebhook",
            null,
            async () => await botClient.SetWebhook(
                options.WebhookUrl.AbsoluteUri,
                allowedUpdates: [global::Telegram.Bot.Types.Enums.UpdateType.Message],
                secretToken: options.WebhookSecret,
                cancellationToken: cancellationToken),
            cancellationToken);
    }

    private async Task ExecuteOutgoingRequest(
        string operation,
        long? messengerId,
        Func<Task> request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
            "Telegram",
            "Outgoing",
            operation,
            messengerId,
            "Started");

        try
        {
            await request();
            logger.LogInformation(
                "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, статус {Status}",
                "Telegram",
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
                "Telegram",
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
                "Telegram",
                "Outgoing",
                operation,
                messengerId,
                "Failed");
            throw;
        }
    }
}
