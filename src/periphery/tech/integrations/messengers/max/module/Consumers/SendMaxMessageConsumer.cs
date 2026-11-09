namespace Tmobis.Integrations.Messengers.Max.Consumers;

using Tmobis.Core.Medium.Consumers;
using Tmobis.Integrations.Messengers.Max.Messages;

public sealed class SendMaxMessageConsumer(
    IMaxAdapter maxAdapter)
    : IMessageConsumer<SendMaxMessage>
{
    public Task Consume(SendMaxMessage message)
        => maxAdapter.SendMessage(message.ChatId, message.Text);
}
