namespace Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Metas;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;

public class DelayedMessageNowConsumer(IMessageProducer producer) : IMessageConsumer<DelayedMessageNow>
{
  public async Task Consume(DelayedMessageNow message)
  {
    if (message.Wrappee is DelayedMessage delayedMessage)
    {
      IMessage? innerMessage = delayedMessage.Wrappee;
      if(innerMessage != null)
        await producer.Send(innerMessage);
    }
  }

}
