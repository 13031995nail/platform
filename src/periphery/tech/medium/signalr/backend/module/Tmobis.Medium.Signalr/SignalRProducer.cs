namespace Tmobis.Medium.Signalr;

using Newtonsoft.Json;
using Tmobis.Core.Medium.Producers;
using Microsoft.AspNetCore.SignalR;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Mappers;
using System;
using Tmobis.Medium.Signalr.Serializers;

public class SignalRProducer : IMessageProducer
{
    private const string ReceiveMethod = "ReceiveMessage";

    private readonly IHubContext<SignalRAdapterHub> context;
    private readonly JsonSerializerSettings jsonSerializerSettings;

    public SignalRProducer(
        IHubContext<SignalRAdapterHub> context,
        IMessageTypeBinder binder)
    {
        this.context = context;
        jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            SerializationBinder = binder,
        };
        RootTypeOnlyContractResolver.ConfigureSerializer(jsonSerializerSettings, includeTypeForRoot: true);
    }

    public Task<IMessageResult> SendWithResult(IMessage message) => throw new InvalidOperationException(
        "В SignalR невозможна отправка с ответом, замените на обычную отправку");

    public async Task Send(IMessage message)
    {
        var convertedMessage = this.SerializeSupportedEvent(message);
        if (convertedMessage == null)
        {
            return;
        }

        if (message?.Context?[SignalRMessageContext.Target] is not string target)
        {
            return;
        }
        await context.Clients.User(target).SendAsync(ReceiveMethod, convertedMessage);
    }

    public async Task Publish(IMessage message)
    {
        var convertedMessage = this.SerializeSupportedEvent(message);
        if (convertedMessage == null)
        {
            return;
        }

        await context.Clients.All.SendAsync(ReceiveMethod, convertedMessage);
    }

    private string? SerializeSupportedEvent(IMessage message)
    {
        if (message is not IDomainEvent && message is not IRemoteEvent)
        {
            return null;
        }

        return JsonConvert.SerializeObject(message, this.jsonSerializerSettings);
    }
}
