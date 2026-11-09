namespace Tmobis.Medium.WebApi.Producer;

using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Mappers;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Producers;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Extensions;

public class WebApiProducer(
    HttpClient httpClient,
    ILogger<WebApiProducer> logger,
    IMessageTypeBinder serializeBinder,
    IOptions<WebApiProducerOptions> options)
    : IMessageProducer
{
    private readonly HttpClient httpClient = httpClient;

    private readonly ILogger<WebApiProducer> logger = logger;

    private readonly WebApiProducerOptions options = options.Value;

    private readonly IMessageTypeBinder serializeBinder = serializeBinder;

    public virtual Task Publish(IMessage message)
    {
        throw new NotImplementedException();
    }

    public virtual async Task Send(IMessage message)
    {
        await SendRequest(message);
    }

    public virtual async Task<IMessageResult> SendWithResult(IMessage message)
    {
        var response = await SendRequest(message);
        var result = await response.Content.ReadAsStringAsync();
        var resultType = message.GetMessageResultType()
            ?? throw new InvalidOperationException("Не удалось найти тип ответа");
        return JsonConvert.DeserializeObject(result, resultType) as IMessageResult
            ?? throw new InvalidOperationException("Не удалось получить ответ");
    }

    protected virtual Task<HttpResponseMessage> SendRequest(IMessage message)
    {
        var metadata = GetMetadata(message);
        var settings = GetSettings(message);
        var json = SerializeRequest(message, settings, metadata);
        var url = GetRequestAddress(message.GetType(), settings, metadata);

        if (message is IQueryMessage)
        {
            return httpClient.GetAsync(BuildQueryRequestAddress(url, json));
        }

        var data = new StringContent(json, Encoding.UTF8, "application/json");
        return httpClient.PostAsync(url, data);
    }

    protected virtual Uri GetRequestAddress(
        Type messageType,
        RemoteApiSettings settings, MessageMetadataAttribute metadata)
    {
        var group = settings.Group ?? metadata.Group;
        var name = settings.Name ?? GetEndpointName(messageType, metadata);
        var groupPath = string.IsNullOrWhiteSpace(group) ? "" : $"/{group}";
        var baseAddress = settings.Address.TrimEnd('/');
        return new($"{baseAddress}{groupPath}/{name}");
    }

    protected virtual Uri BuildQueryRequestAddress(Uri url, string messageJson)
    {
        var separator = string.IsNullOrEmpty(url.Query) ? "?" : "&";
        return new Uri(
            $"{url}{separator}message={Uri.EscapeDataString(messageJson)}");
    }

    protected virtual string GetEndpointName(
        Type messageType,
        MessageMetadataAttribute metadata)
    {
        if (!string.IsNullOrWhiteSpace(metadata.Name))
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

    protected virtual string SerializeRequest(IMessage message,
        RemoteApiSettings settings, MessageMetadataAttribute metadata)
    {
        JsonSerializerSettings serializeSettings = settings.IsInternal
            ? new() { SerializationBinder = serializeBinder }
            : new();
        return JsonConvert.SerializeObject(message, serializeSettings);
    }

    protected virtual RemoteApiSettings GetSettings(IMessage message)
    {
        var type = message.GetType();
        options.Types.TryGetValue(type.Name, out var apiSettings);
        if (apiSettings is null || string.IsNullOrEmpty(apiSettings.Address))
        {
            throw new InvalidOperationException(
                "Не найден адрес в конфигурации");
        }

        return apiSettings;
    }

    protected virtual MessageMetadataAttribute GetMetadata(IMessage message)
    => message.GetType().GetCustomAttribute<MessageMetadataAttribute>()
            ?? throw new InvalidOperationException(
                "Не указаны метаданные сообщения");
}
