namespace Tmobis.Core.Medium.Listeners;

using Microsoft.Extensions.Hosting;

public interface IMessageListenerWorker : IMessageListener, IHostedService
{
}
