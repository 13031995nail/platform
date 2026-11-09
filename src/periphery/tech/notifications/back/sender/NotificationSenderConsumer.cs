namespace Tmobis.Notification.Sender;

using System;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;
using Tmobis.Notification.Models;

public class NotificationSenderConsumer(
    IRepository repository,
    IUnitOfWork unitOfWork,
    IEnumerable<INotificationProvider> providers
) : IMessageConsumer<CreateNotification>
{
    public async Task Consume(CreateNotification msg)
    {
        var user = await repository.GetById<IUser>(msg.UserId)
            ?? throw new InvalidOperationException("Не найден пользователь");
        var type = await repository.GetById<INotificationType>(msg.TypeId)
            ?? throw new InvalidOperationException("Не найден тип уведомления");
        var providerIds = msg.Content.Select(x => x.ProviderTypeId).Distinct().ToList();
        var notification = repository.MakeEntity<INotification>();
        notification.Status = NotificationStatus.Created;
        notification.User = user;
        notification.Type = type;
        repository.Create(notification);
        foreach (var content in msg.Content)
        {
            var notificationContent = repository.MakeEntity<INotificationContent>();
            notificationContent.ProviderTypeId = content.ProviderTypeId;
            notificationContent.Title = content.Title;
            notificationContent.Content = content.Content;
            notificationContent.Notification = notification;
            repository.Create(notificationContent);
        }
        await unitOfWork.Commit();
        foreach (var provider in providers)
        {
            await provider.SendNotification(notification);
        }
    }
}

