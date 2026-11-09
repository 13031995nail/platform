namespace Tmobis.Core.Medium.Listeners;

public class MessageListenerWorker(
    IEnumerable<IHostedMessageListener> hostedMessageListeners)
: IMessageListenerWorker
{
    private bool disposed;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var hostedMessageListener in hostedMessageListeners)
        {
            hostedMessageListener.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var hostedMessageListener in hostedMessageListeners)
        {
            await hostedMessageListener.StopAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed) return;
        if (disposing)
        {
            foreach (var hostedMessageListener in hostedMessageListeners)
            {
                hostedMessageListener.Dispose();
            }
        }

        disposed = true;
    }
}
