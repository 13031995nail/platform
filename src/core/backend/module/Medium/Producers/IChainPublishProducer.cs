namespace Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Models;

public interface IChainPublishProducer
{
    Task<List<IChainMessageResult>> PublishChainWithResult(IMessage message);
}
