namespace Tmobis.Medium.InRepo;

using Tmobis.Core.Medium.Models;

public interface IRepositoryProducer
{
  IStoredMessageEntity MapMessageToEntity(IMessage message);
  void WriteToRepository(IStoredMessageEntity entity);
}
