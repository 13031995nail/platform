namespace Tmobis.Integrations.Messengers.Vk.Consumers;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Messages;
using Tmobis.Integrations.Messengers.Vk.Messages;

public sealed class VkCallbackReceivedConsumer(
    IMessageDispatcher messageDispatcher,
    IVkAdapter vkAdapter,
    ILogger<VkCallbackReceivedConsumer> logger)
    : IMessageConsumer<VkCallbackReceived>
{
    public async Task Consume(VkCallbackReceived callback)
    {
        var message = callback.Object?.Message;
        var peerId = message?.PeerId;
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, событие {EventId}",
            "VK",
            "Incoming",
            callback.Type,
            peerId,
            callback.EventId);

        if (!string.Equals(
            callback.Type,
            "message_new",
            StringComparison.Ordinal)
            || message == null
            || message.Out != 0)
        {
            return;
        }

        if (peerId is null or 0 || message.FromId != peerId)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, событие {EventId} получено не из личного диалога",
                "VK",
                "Incoming",
                callback.EventId);
            return;
        }

        var phone = ExtractPhone(message.Text);
        var result = await messageDispatcher.SendWithResult<
            MessengerMessageReceived,
            MessengerMessageResult>(new MessengerMessageReceived
            {
                MessengerCode = VkOptions.MessengerCode,
                MessengerName = VkOptions.MessengerName,
                ExternalId = peerId.Value.ToString(
                    CultureInfo.InvariantCulture),
                HasContact = phone != null,
                ContactIsTrusted = false,
                AllowUntrustedContact = true,
                Phone = phone
            });

        switch (result.Action)
        {
            case MessengerResponseAction.RequestContact:
                await vkAdapter.SendMessage(
                    peerId.Value,
                    "Отправьте номер телефона сотрудника.");
                break;
            case MessengerResponseAction.SendMessage
                when !string.IsNullOrWhiteSpace(result.Text):
                await vkAdapter.SendMessage(peerId.Value, result.Text);
                break;
        }
    }

    internal static string? ExtractPhone(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var digits = new string(text.Where(char.IsDigit).ToArray());
        if (digits.Length == 10)
        {
            return digits;
        }

        return digits.Length == 11 && digits[0] is '7' or '8'
            ? digits
            : null;
    }
}
