namespace Tmobis.Integrations.Messengers.Vk.Consumers;

using Tmobis.Core.Medium.Consumers;
using Tmobis.Integrations.Messengers.Vk.Messages;

public sealed class SendVkMessageConsumer(
    IVkAdapter vkAdapter)
    : IMessageConsumer<SendVkMessage>
{
    public Task Consume(SendVkMessage message)
        => vkAdapter.SendMessage(message.PeerId, message.Text);
}
