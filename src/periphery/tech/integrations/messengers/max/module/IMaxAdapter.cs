namespace Tmobis.Integrations.Messengers.Max;

public interface IMaxAdapter
{
    Task SendMessage(
        long chatId,
        string message,
        CancellationToken cancellationToken = default);

    Task RequestContact(
        long chatId,
        CancellationToken cancellationToken = default);

    Task RegisterWebhook(CancellationToken cancellationToken = default);
}
