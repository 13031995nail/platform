namespace Tmobis.Medium.Delayed.InRepo;
using Tmobis.Core.Medium.Delayed;
using Core.Repositories;
using Tmobis.Medium.InRepo;
using Newtonsoft.Json;

public class DelayedMessageCreatedRepositoryProducer(
    IRepository repository)
: RepositoryProducer<DelayedMessageCreated, DelayedMessageEntity>(repository)
{
    private readonly IRepository repository = repository;

    protected override DelayedMessageEntity MapMessageToEntity(DelayedMessageCreated message)
  {
    var entity = repository.MakeEntity<DelayedMessageEntity>();
    repository.Create(entity);
    if (message.Wrappee is not IDelayedMessage delayedMessage)
        return entity;
    
    var settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        Formatting = Formatting.None
    };

    var serialized = JsonConvert.SerializeObject(delayedMessage.Wrappee, settings);
    entity.Time = delayedMessage.Time;
    entity.RawMessage = serialized;
    return entity;
  }
}
