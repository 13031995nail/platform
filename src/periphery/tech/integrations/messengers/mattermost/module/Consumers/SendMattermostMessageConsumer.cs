namespace Tmobis.Integrations.Messengers.Mattermost.Consumers;

using Tmobis.Core.Medium.Consumers;
using Tmobis.Integrations.Messengers.Mattermost.Messages;

public sealed class SendMattermostMessageConsumer(
    IMattermostAdapter mattermostAdapter)
    : IMessageConsumer<SendMattermostMessage>
{
    public Task Consume(SendMattermostMessage message)
        => mattermostAdapter.SendMessage(message.UserId, message.Text);
}
