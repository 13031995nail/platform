namespace Tmobis.Business.Common.Sessions.Consumers;
using Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Messages;
using Tmobis.Core.Medium.Consumers;
using System.Threading.Tasks;
using Tmobis.Core.Repositories;

public class UserSessionSetDeviceDataConsumer(
    IUnitOfWork unitOfWork,
    IUserSessionProvider sessionProvider)
: IMessageConsumer<UserSessionSetDeviceData>
{
    public async Task Consume(UserSessionSetDeviceData message)
    {
        var session = await sessionProvider.GetCurrentSession();
        if (session == null)
        {
            return;
        }

        session.DeviceInfo = message.DeviceInfo;
        await unitOfWork.Commit();
    }
}
