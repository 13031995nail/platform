namespace Tmobis.Core.Medium.Models;

public interface IRemoteEvent
    : IEventMessage;

public abstract record RemoteEvent : Message, IRemoteEvent;

public interface IRemoteQuery<TResult>
    : IMessage<TResult>, IQueryMessage
    where TResult : IMessageResult;

public abstract record RemoteQuery<TResult>
    : Message, IRemoteQuery<TResult>
    where TResult : IMessageResult;

public interface IRemoteCommand<TResult>
    : IMessage<TResult>, ICommandMessage
    where TResult : IMessageResult;
    
public abstract record RemoteCommand<TResult>
    : Message, IRemoteCommand<TResult>
    where TResult : IMessageResult;