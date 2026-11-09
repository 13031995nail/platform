namespace Tmobis.Business.Common.Sessions.Consumers;

using System.Security.Claims;
using Auth;
using Providers;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;

public class OnPrepareAuthTokenSession(
    IUnitOfWork unitOfWork,
    IUserSessionProvider sessionProvider)
: IMessageConsumer<PrepareAuthToken, PrepareAuthTokenResult>
{
    public async Task<PrepareAuthTokenResult>
        ConsumeWithResult(PrepareAuthToken message)
    {
        var user = message.User;
        var session = sessionProvider.CreateByUser(user);
        await unitOfWork.Commit();
        return new(
            [new (ClaimTypes.UserData, session.Id.ToString())],
            new Dictionary<string, string>
            {
                ["sessionId"] = session.Id.ToString()
            }
        )
        { Continue = true };
    }
}
