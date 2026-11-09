namespace Tmobis.Medium.WebApi;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using Core.Medium.Extensions;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;

public class MediumEndpointBuilder(
    IMessageTypeBinder binder,
    IMessageListRegister listenedRegister)
: ApiEndpointBuilder
{
    private readonly JsonSerializerSettings jsonSettings
    = new() { SerializationBinder = binder };
    private readonly JsonSerializerOptions resultJsonSettings
    = new(JsonSerializerDefaults.Web)
    { ReferenceHandler = ReferenceHandler.IgnoreCycles };
    private readonly IReadOnlyList<Type> listened
    = listenedRegister.GetListenedMessageList("WebApi");

    public override void Build(IEndpointRouteBuilder endpoints)
    {
        foreach (var messageType in listened)
        {
            ConfigureMessageEndpoint(messageType, endpoints);
        }
    }

    protected virtual void ConfigureMessageEndpoint(
        Type type,
        IEndpointRouteBuilder endpoints)
    {
        var meta = type
            .GetCustomAttribute<MessageMetadataAttribute>();
        var isQuery = type.IsAssignableTo(typeof(IQueryMessage));
        var endpointName = GetEndpointName(type, meta);
        var endpoint = BuildEndpoint(endpointName, meta?.Group);

        RouteHandlerBuilder? endpointBuilder;
        if (isQuery)
        {
            endpointBuilder = endpoints.MapGet(endpoint,
                async (
                    string messageJson,
                    IMessageDispatcher dispatcher,
                    IUnitOfWork unitOfWork) =>
                {
                    var deserializedMessage = DeserializeMessage(messageJson, type);
                    var result = await SendMessageToDispatcher(
                        dispatcher,
                        deserializedMessage);
                    await unitOfWork.Commit();
                    return ToJsonResult(result);
                });
        }
        else
        {
            endpointBuilder = endpoints.MapPost(endpoint,
                async (
                    HttpRequest request,
                    IMessageDispatcher dispatcher,
                    IUnitOfWork unitOfWork) =>
                {
                    var message = await ParsePostMessage(request, type);
                    var result = await SendMessageToDispatcher(dispatcher, message);
                    await unitOfWork.Commit();
                    return ToJsonResult(result);
                });
        }

        if (!string.IsNullOrEmpty(meta?.Group))
        {
            endpointBuilder
                .WithTags(meta.Group);
        }

        ConfigureAuthorization(endpointBuilder, type);

        ConfigureRequestBody(endpointBuilder, type, isQuery);
    }

    private static void ConfigureAuthorization(
        RouteHandlerBuilder endpointBuilder,
        Type type)
    {
        if (type.IsDefined(typeof(AllowAnonymousAttribute), true))
        {
            endpointBuilder.AllowAnonymous();
            return;
        }

        var authorizeData = type
            .GetCustomAttributes<AuthorizeAttribute>(true)
            .Cast<IAuthorizeData>()
            .ToArray();
        if (authorizeData.Length > 0)
        {
            endpointBuilder.RequireAuthorization(authorizeData);
            return;
        }

        endpointBuilder.RequireAuthorization();
    }

    protected virtual async Task<IMessage> ParsePostMessage(
        HttpRequest request, Type type)
    {
        using var stream = new StreamReader(request.Body);
        var messageJson = await stream.ReadToEndAsync();
        return DeserializeMessage(messageJson, type);
    }

    protected virtual string GetEndpointName(
        Type messageType,
        MessageMetadataAttribute? metadata)
    {
        if (!string.IsNullOrWhiteSpace(metadata?.Name))
        {
            return metadata.Name;
        }

        var messageTypeName = messageType.IsGenericType
            ? messageType.Name.Split('`')[0]
            : messageType.Name;
        if (!messageType.IsGenericType)
        {
            return messageTypeName;
        }

        var genericArguments = string.Join(
            "/",
            messageType.GenericTypeArguments.Select(type => type.Name));
        return $"{messageTypeName}/{genericArguments}";
    }

    protected virtual void ConfigureRequestBody(
        RouteHandlerBuilder endpointBuilder,
        Type type,
        bool isQuery)
    {
        if (!isQuery)
        {
            endpointBuilder.Accepts(type, "application/json");
        }
    }

    protected virtual IMessage DeserializeMessage(string messageJson, Type type)
    => (IMessage?)JsonConvert.DeserializeObject(
        messageJson, type, jsonSettings)
        ?? throw new InvalidCastException(
            "Не удалось получить сообщение");

    protected virtual IResult ToJsonResult(IMessageResult? result)
    => result == null
        ? Results.NoContent()
        : Results.Json(result, resultJsonSettings);

    protected virtual async ValueTask<IMessageResult?> SendMessageToDispatcher(
        IMessageDispatcher dispatcher,
        IMessage deserializedMessage)
    {
        var resultType = MessageExtensions
            .GetMessageResultType(deserializedMessage.GetType());
        if (resultType != null)
        {
            return await dispatcher
                .SendWithResult(deserializedMessage);
        }

        await dispatcher.Send(deserializedMessage);
        return null;
    }
}
