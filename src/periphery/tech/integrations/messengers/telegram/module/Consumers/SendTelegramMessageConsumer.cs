namespace Tmobis.Integrations.Messengers.Telegram.Consumers;

using Tmobis.Core.Medium.Consumers;
using Tmobis.Integrations.Messengers.Telegram.Messages;

public sealed class SendTelegramMessageConsumer(
    ITelegramAdapter telegramAdapter)
    : IMessageConsumer<SendTelegramMessage>
{
    public Task Consume(SendTelegramMessage message)
        => telegramAdapter.SendMessage(message.ChatId, message.Text);
}
