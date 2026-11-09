namespace Tmobis.Core.Medium.Models;

public interface IDomainEvent
    : IAmbientScopeMessage, IEventMessage;

public abstract record DomainEvent : Message, IDomainEvent;

public interface IDomainQuery<TResult>
    : IMessage<TResult>, IAmbientScopeMessage, IQueryMessage
    where TResult : IMessageResult;

public abstract record DomainQuery<TResult>
    : Message, IDomainQuery<TResult>
    where TResult : IMessageResult;

public interface IDomainCommand<TResult>
    : IMessage<TResult>, IAmbientScopeMessage, ICommandMessage
    where TResult : IMessageResult;
    
public abstract record DomainCommand<TResult>
    : Message, IDomainCommand<TResult>
    where TResult : IMessageResult;