namespace Tmobis.Business.Common.Sessions.Consumers;
using Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Messages;
using Tmobis.Core.Medium.Consumers;
using System.Threading.Tasks;
using Tmobis.Core.Repositories;

public class UserSessionExpireAllConsumer(
    IUnitOfWork unitOfWork,
    IUserSessionProvider sessionProvider)
: IMessageConsumer<UserSessionExpireAll>
{
    public async Task Consume(UserSessionExpireAll message)
    {
        await sessionProvider.ExpireAll();
        await unitOfWork.Commit();
    }
}
