namespace Tmobis.Integrations.Messengers.Telegram.Consumers;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Messages;
using Tmobis.Integrations.Messengers.Telegram.Messages;

public sealed class TelegramUpdateReceivedConsumer(
    IMessageDispatcher messageDispatcher,
    ITelegramAdapter telegramAdapter,
    ILogger<TelegramUpdateReceivedConsumer> logger)
    : IMessageConsumer<TelegramUpdateReceived>
{
    public async Task Consume(TelegramUpdateReceived update)
    {
        var chatId = update.Message?.Chat?.Id;
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, событие {EventId}",
            "Telegram",
            "Incoming",
            "UpdateReceived",
            chatId,
            update.UpdateId);

        if (chatId is null or 0)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, событие {EventId} не содержит идентификатор",
                "Telegram",
                "Incoming",
                update.UpdateId);
            return;
        }

        var contact = update.Message?.Contact;
        var result = await messageDispatcher.SendWithResult<
            MessengerMessageReceived,
            MessengerMessageResult>(new MessengerMessageReceived
            {
                MessengerCode = TelegramOptions.MessengerCode,
                MessengerName = TelegramOptions.MessengerName,
                ExternalId = chatId.Value.ToString(
                    CultureInfo.InvariantCulture),
                HasContact = contact != null,
                ContactIsTrusted = contact?.UserId is long contactUserId
                    && update.Message?.From?.Id == contactUserId,
                Phone = contact?.PhoneNumber
            });

        switch (result.Action)
        {
            case MessengerResponseAction.RequestContact:
                await telegramAdapter.RequestContact(chatId.Value);
                break;
            case MessengerResponseAction.SendMessage
                when !string.IsNullOrWhiteSpace(result.Text):
                await telegramAdapter.SendMessage(chatId.Value, result.Text);
                break;
        }
    }
}
