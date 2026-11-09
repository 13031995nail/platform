namespace Tmobis.Medium.Signalr;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;

public class SignalRAdapterHub : BaseHub, IMessageListener
{
    private readonly IMessageDispatcher dispatcher;

    private readonly IReadOnlyList<Type> listenedMessageList;

    private readonly JsonSerializerSettings jsonSerializerSettings;

    private readonly SignalRConnections connections;

    private readonly ILogger<SignalRAdapterHub> logger;

    private readonly IMessageListRegister messageListRegister;

    public SignalRAdapterHub(
        IMessageDispatcher dispatcher,
        IMessageTypeBinder binder,
        SignalRConnections connections,
        ILogger<SignalRAdapterHub> logger,
        IMessageListRegister messageListRegister)
    {
        this.dispatcher = dispatcher;
        this.connections = connections;
        this.messageListRegister = messageListRegister;
        this.listenedMessageList = messageListRegister.GetListenedMessageList("SignalR");
        this.logger = logger;
        this.jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            SerializationBinder = binder
        };
    }

    public override async Task OnConnectedAsync()
    {
        var target = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(target))
        {
            connections.TryAdd(target, true);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var target = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(target))
        {
            connections.TryRemove(target, out _);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message)
    {
        var target = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(target))
        {
            return;
        }

        var deserializedMessage = this.DeserializeMessage(message);
        if (deserializedMessage == null)
        {
            return;
        }

        deserializedMessage.Context[SignalRMessageContext.Target] = target;

        await this.SendMessageToDispatcher(deserializedMessage, this.dispatcher);
    }

    public async Task PublishMessage(string message)
    {
        var target = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(target))
        {
            return;
        }

        var deserializedMessage = this.DeserializeMessage(message);
        if (deserializedMessage == null)
        {
            return;
        }

        deserializedMessage.Context[SignalRMessageContext.Target] = target;

        await this.PublishMessageToDispatcher(deserializedMessage, this.dispatcher);
    }

    protected virtual IMessage? DeserializeMessage(string message)
    {
        if (JsonConvert.DeserializeObject(message, this.jsonSerializerSettings) is not IMessage deserializedMessage
            || !this.listenedMessageList.Contains(deserializedMessage.GetType()))
        {
            return null;
        }

        if (deserializedMessage is not IDomainEvent
            && deserializedMessage is not IRemoteEvent)
        {
            this.logger.LogWarning(
                "SignalR отклонил сообщение недопустимого типа {MessageType}",
                deserializedMessage.GetType().FullName);
            return null;
        }

        return deserializedMessage;
    }

    protected virtual async Task SendMessageToDispatcher(IMessage? deserializedMessage, IMessageDispatcher dispatcher)
    {
        if (deserializedMessage == null)
        {
            return;
        }

        if (this.CheckForGeneric(deserializedMessage))
        {
            await dispatcher.SendWithResult(deserializedMessage);
        }
        await dispatcher.Send(deserializedMessage);
    }

    protected virtual bool CheckForGeneric(IMessage message)
    {
        var interfaces = message.GetType().GetInterfaces();
        var genericMessageInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType
            && i.IsAssignableTo(typeof(IMessage))
            && i.GenericTypeArguments[0]
                  .IsAssignableTo(typeof(IMessageResult)));

        return genericMessageInterface != null;
    }

    protected virtual async Task PublishMessageToDispatcher(IMessage? deserializedMessage, IMessageDispatcher dispatcher)
    {
        if (deserializedMessage == null)
        {
            return;
        }
        await dispatcher.Publish(deserializedMessage);
    }
}

