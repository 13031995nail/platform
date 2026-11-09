namespace Tmobis.Core.Medium.Listeners;

public interface IHostedMessageListener : IMessageListener
{
    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);
}
