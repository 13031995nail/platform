namespace Tmobis.Jobs.Scheduled;
using Tmobis.Core.Modules;

public interface IScheduledMessagesModule : IModule
{
    List<Type> GetScheduledMessages();
}
