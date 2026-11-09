namespace Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Models;

public interface IMessageProducer
{
    Task<IMessageResult> SendWithResult(IMessage message);

    Task Send(IMessage message);

    Task Publish(IMessage message);
}
