namespace Tmobis.Medium.Retried.WithDelay;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Retried.Models;
using Tmobis.Medium.Retried.RabbitMQ.PrimaryAdapters;
using Microsoft.Extensions.Options;

public class RetriedMessageConsumer(IMessageDispatcher messageDispatcher, IOptions<DLQRabbitListenerOptions> options) : IMessageConsumer<RetriedMessageCreated>
{
  private readonly DLQRabbitListenerOptions options = options.Value;
  public async Task Consume(RetriedMessageCreated message)
  {
    object tryCountObj = message.Wrappee.Context[RetriedMessageContext.TryCount];
    int? tryCount = tryCountObj != null ? Convert.ToInt32(tryCountObj) : (int?)null;
    int? maxCount = message.Wrappee.Context[RetriedMessageContext.MaxTryCount] as int?;
    if (tryCount > maxCount)
    {
      return;
    }

    var delayedMessage = new DelayedMessage { Wrappee = message.Wrappee, Time = DateTime.UtcNow.AddSeconds(options.DelayedTime) };
    var delayedMessageCreated = new DelayedMessageCreated { Wrappee = delayedMessage };
    messageDispatcher.Send(delayedMessageCreated);
  }
}

