namespace Tmobis.Integrations.Messengers.Mattermost.Consumers;

using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Messages;
using Tmobis.Integrations.Messengers.Mattermost.Messages;

public sealed class MattermostPostReceivedConsumer(
    IMessageDispatcher messageDispatcher,
    IMattermostAdapter mattermostAdapter,
    ILogger<MattermostPostReceivedConsumer> logger)
    : IMessageConsumer<MattermostPostReceived>
{
    public async Task Consume(MattermostPostReceived post)
    {
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, событие {EventId}",
            "Mattermost",
            "Incoming",
            "PostReceived",
            post.UserId,
            post.PostId);

        var phone = ExtractPhone(post.Text);
        var result = await messageDispatcher.SendWithResult<
            MessengerMessageReceived,
            MessengerMessageResult>(new MessengerMessageReceived
            {
                MessengerCode = MattermostOptions.MessengerCode,
                MessengerName = MattermostOptions.MessengerName,
                ExternalId = post.UserId,
                HasContact = phone != null,
                ContactIsTrusted = false,
                AllowUntrustedContact = true,
                Phone = phone
            });

        switch (result.Action)
        {
            case MessengerResponseAction.RequestContact:
                await mattermostAdapter.SendMessage(
                    post.UserId,
                    "Отправьте номер телефона сотрудника.");
                break;
            case MessengerResponseAction.SendMessage
                when !string.IsNullOrWhiteSpace(result.Text):
                await mattermostAdapter.SendMessage(post.UserId, result.Text);
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
