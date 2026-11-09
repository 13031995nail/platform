namespace Tmobis.Integrations.Messengers.Consumers;

using Microsoft.Extensions.Logging;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;
using Tmobis.Integrations.Messengers.Messages;
using Tmobis.Integrations.Messengers.Providers;

public sealed class MessengerMessageReceivedConsumer(
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IUnitOfWork unitOfWork,
    ILogger<MessengerMessageReceivedConsumer> logger)
    : IMessageConsumer<MessengerMessageReceived, MessengerMessageResult>
{
    public async Task<MessengerMessageResult> ConsumeWithResult(
        MessengerMessageReceived message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message.MessengerCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(message.MessengerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(message.ExternalId);

        var registeredEmployee = await employeeMessengerProvider
            .GetByExternalId(message.MessengerCode, message.ExternalId);
        if (registeredEmployee != null)
        {
            return Reply("Вы уже зарегистрированы.");
        }

        if (!message.HasContact)
        {
            return new MessengerMessageResult
            {
                Action = MessengerResponseAction.RequestContact
            };
        }

        if (!message.ContactIsTrusted && !message.AllowUntrustedContact)
        {
            logger.LogWarning(
                "Контакт {Messenger} для пользователя {ExternalId} не принадлежит отправителю",
                message.MessengerName,
                message.ExternalId);
            return Reply(
                "Не удалось подтвердить контакт. Используйте кнопку "
                + "«Поделиться контактом».");
        }

        var phone = NormalizePhone(message.Phone);
        if (phone == null)
        {
            return Reply(
                "В переданном контакте не найден корректный номер телефона.");
        }

        var employees = await employeeProvider.GetWithPhone();
        var matchedEmployees = employees
            .Where(employee => NormalizePhone(employee.Phone) == phone)
            .ToList();
        if (matchedEmployees.Count != 1)
        {
            logger.LogWarning(
                "По контакту {Messenger} найдено сотрудников: {EmployeeCount}",
                message.MessengerName,
                matchedEmployees.Count);
            return Reply(
                matchedEmployees.Count == 0
                    ? "Сотрудник с указанным номером телефона не найден."
                    : "Найдено несколько сотрудников с указанным номером телефона.");
        }

        var link = await employeeMessengerProvider.Link(
            matchedEmployees[0],
            message.MessengerCode,
            message.ExternalId);
        await employeeMessengerProvider.SetPreferredMessengerIfMissing(link);
        await unitOfWork.Commit();

        return Reply(
            $"Регистрация в {message.MessengerName} успешно завершена.");
    }

    private static MessengerMessageResult Reply(string text)
        => new()
        {
            Action = MessengerResponseAction.SendMessage,
            Text = text
        };

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length >= 10 ? digits[^10..] : null;
    }
}
