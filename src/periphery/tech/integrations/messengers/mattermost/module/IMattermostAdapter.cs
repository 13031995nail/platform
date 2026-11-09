namespace Tmobis.Integrations.Messengers.Mattermost;

public interface IMattermostAdapter
{
    Task SendMessage(
        string userId,
        string message,
        CancellationToken cancellationToken = default);
}
