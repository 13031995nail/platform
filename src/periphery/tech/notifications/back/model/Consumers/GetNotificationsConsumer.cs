namespace Tmobis.Notification.Consumers;

using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;
using Tmobis.Notification.Messages;
using Tmobis.Notification.Models;

public sealed class GetNotificationsConsumer(
    IRepository repository,
    ICurrentUserProvider currentUserProvider)
    : IMessageConsumer<
        GetNotifications,
        EntityListQueryResult<INotification>>
{
    private static readonly EntityView NotificationListView = new(
        nameof(NotificationListView),
        [
            nameof(INotification.Type),
            nameof(INotification.NotificationContents),
        ]);

    public async Task<EntityListQueryResult<INotification>> ConsumeWithResult(
        GetNotifications message)
    {
        var currentUser = await currentUserProvider.GetCurrentUser()
            ?? throw new InvalidOperationException("Не найден пользователь");

        var result = await EntityListQueryHelper.GetList(
            repository,
            message.Page,
            message.PerPage,
            GetNotifications.DefaultPerPage,
            GetNotifications.MaxPerPage,
            orderBy: query => query.OrderByDescending(
                notification => notification.CreateTime),
            includeTotalCount: true,
            predicate: (INotification notification) =>
                notification.UserId == currentUser.Id,
            view: NotificationListView);
        await MarkUnreadNotificationsAsRead(currentUser.Id);

        return result;
    }

    private async Task MarkUnreadNotificationsAsRead(Guid userId)
    {
        var unreadNotifications = await repository.Find<INotification>(
            notification => notification.UserId == userId
                && notification.Status != NotificationStatus.Read);
        var readingTime = DateTime.UtcNow;

        foreach (var notification in unreadNotifications)
        {
            notification.Status = NotificationStatus.Read;
            notification.ReadingTime = readingTime;
            repository.Update(notification);
        }
    }
}
