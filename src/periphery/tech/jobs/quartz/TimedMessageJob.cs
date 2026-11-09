namespace Tmobis.Jobs.Quartz;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Models;
using System.Threading.Tasks;
using global::Quartz;

[DisallowConcurrentExecution]
public class TimedMessageJob<T>(IMessageDispatcher messageDispatcher) : IJob
where T : class, IMessage, new()
{
    public async Task Execute(IJobExecutionContext context)
    {
        await messageDispatcher.Publish(new T());
    }
}
