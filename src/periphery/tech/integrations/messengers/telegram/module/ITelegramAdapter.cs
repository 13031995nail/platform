namespace Tmobis.Integrations.Messengers.Telegram;

public interface ITelegramAdapter
{
    Task SendMessage(
        long chatId,
        string text,
        CancellationToken cancellationToken = default);

    Task RequestContact(
        long chatId,
        CancellationToken cancellationToken = default);

    Task RegisterWebhook(CancellationToken cancellationToken = default);
}
