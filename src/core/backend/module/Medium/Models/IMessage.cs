namespace Tmobis.Core.Medium.Models;

public interface IMessage
{
    Dictionary<string, object?> Context { get; }
}

public interface IMessageResult;

public interface IMessage<TResult> : IMessage
    where TResult : IMessageResult;

/// <summary>
/// Используется только как маркер для эффективного определения типа сообщения.
/// </summary>
public interface IQueryMessage
    : IMessage;

/// <summary>
/// Используется только как маркер для эффективного определения типа сообщения.
/// </summary>
public interface ICommandMessage
    : IMessage;

/// <summary>
/// Используется только как маркер для эффективного определения типа сообщения.
/// </summary>
public interface IEventMessage
    : IMessage;

public abstract record Message : IMessage
{
    public Dictionary<string, object?> Context { get; init; } = [];
}

public abstract record Message<TResult>
    : Message, IMessage<TResult>
    where TResult : IMessageResult;