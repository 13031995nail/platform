namespace Tmobis.Business.Common.Sessions.Consumers;
using Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Messages;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Medium.Consumers;
using System.Threading.Tasks;
using Tmobis.Core.Repositories;

public class UserSessionStopSessionConsumer(
    IUnitOfWork unitOfWork,
    IUserProvider userProvider,
    IUserSessionProvider sessionProvider,
    ICurrentUserProvider currentUserProvider)
: IMessageConsumer<UserSessionStopSession>
{
    public async Task Consume(UserSessionStopSession message)
    {
        var sessionId = message.SessionId;
        if (sessionId.HasValue)
        {
            var currentUser = await currentUserProvider.GetCurrentUser();
            if (currentUser == null)
            {
                return;
            }

            var session = await sessionProvider.GetById(sessionId.Value);
            if (session?.User == null)
            {
                return;
            }

            var sessionUser = await userProvider.GetUserById(session.User.Id)
                ?? throw new InvalidOperationException("Не удалось определить пользователя");
            var canDeleteSession = await userProvider
                .IsSameUserIdOrAdmin(currentUser, sessionUser);

            if (!canDeleteSession)
            {
                return;
            }

            sessionProvider.Expire(session);
        }
        else
        {
            sessionId = sessionProvider.GetCurrentSessionId();
            if (!sessionId.HasValue)
            {
                return;
            }

            var session = await sessionProvider.GetById(sessionId.Value)
                ?? throw new InvalidOperationException(
                    "Не найдена сессия");
            sessionProvider.Expire(session);
        }

        await unitOfWork.Commit();
    }
}
