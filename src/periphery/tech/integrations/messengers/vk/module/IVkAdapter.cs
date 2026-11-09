namespace Tmobis.Integrations.Messengers.Vk;

public interface IVkAdapter
{
    Task SendMessage(
        long peerId,
        string message,
        CancellationToken cancellationToken = default);
}
