namespace Tmobis.Notification.Consumers;

using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;
using Tmobis.Notification.Messages;
using Tmobis.Notification.Models;

public sealed class GetUnreadNotificationCountConsumer(
    IRepository repository,
    ICurrentUserProvider currentUserProvider)
    : IMessageConsumer<
        GetUnreadNotificationCount,
        GetUnreadNotificationCountResult>
{
    public async Task<GetUnreadNotificationCountResult> ConsumeWithResult(
        GetUnreadNotificationCount message)
    {
        var currentUser = await currentUserProvider.GetCurrentUser();
        if (currentUser == null)
        {
            return new GetUnreadNotificationCountResult();
        }

        var count = await repository.Count<INotification>(
            notification =>
                notification.UserId == currentUser.Id
                && notification.Status != NotificationStatus.Read);

        return new GetUnreadNotificationCountResult { Count = count };
    }
}
