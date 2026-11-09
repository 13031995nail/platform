namespace Tmobis.Medium.InRepo;

using Tmobis.Core.Medium.Producers;
using Core.Repositories;
using Tmobis.Core.Medium.Models;

public abstract class RepositoryProducer<TMessage, TStoredMessageEntity> (
    IRepository repository)
: IMessageProducer, IRepositoryProducer
    where TStoredMessageEntity : class, IStoredMessageEntity
    where TMessage : class, IMessage
{
    public Task Publish(IMessage message)
    {
        throw new NotImplementedException();
    }

    public Task<IMessageResult> SendWithResult(IMessage message)
    {
        throw new NotImplementedException();
    }

    public Task Send(IMessage message)
    {
        if (message is not TMessage typedMessage)
            throw new ArgumentException(
                $"Ожидалось сообщение типа {typeof(TMessage).Name}");
        var entity = MapMessageToEntity(typedMessage);
        WriteToRepository(entity);
        return Task.CompletedTask;
    }

    IStoredMessageEntity IRepositoryProducer.MapMessageToEntity(IMessage message)
    => MapMessageToEntity((TMessage)message);

    void IRepositoryProducer.WriteToRepository(IStoredMessageEntity entity)
    => WriteToRepository((TStoredMessageEntity)entity);

    protected abstract TStoredMessageEntity MapMessageToEntity(TMessage message);

    protected virtual void WriteToRepository(TStoredMessageEntity entity)
    => repository.Update(entity);
}
