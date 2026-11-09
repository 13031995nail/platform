namespace Tmobis.Cache.Redis;

using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

internal sealed class RedisCacheSerializer(
  IEntityMaterializer entityMaterializer,
  IMessageTypeBinder messageTypeBinder)
{
  private readonly JsonSerializerSettings defaultSerializerSettings =
    CreateBaseSettings();

  private readonly JsonSerializerSettings entitySerializerSettings =
    CreateEntitySettings(entityMaterializer);

  private readonly JsonSerializerSettings messageSerializerSettings =
    CreateMessageSettings(messageTypeBinder);

  public byte[] Serialize<T>(T value)
  {
    var payload = Serialize(
      value,
      typeof(T),
      GetSerializerSettings<T>());
    return Encoding.UTF8.GetBytes(payload);
  }

  public T? Deserialize<T>(byte[] bytes)
  {
    var payload = Encoding.UTF8.GetString(bytes);
    return JsonConvert.DeserializeObject<T>(
      payload,
      GetSerializerSettings<T>());
  }

  private JsonSerializerSettings GetSerializerSettings<T>()
    => GetSerializerSettings(typeof(T));

  private JsonSerializerSettings GetSerializerSettings(Type type)
  {
    if (typeof(IMessage).IsAssignableFrom(type))
    {
      return messageSerializerSettings;
    }

    if (typeof(IEntity).IsAssignableFrom(type))
    {
      return entitySerializerSettings;
    }

    return defaultSerializerSettings;
  }

  private static JsonSerializerSettings CreateBaseSettings()
    => new()
    {
      MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
      PreserveReferencesHandling = PreserveReferencesHandling.Objects,
      ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
    };

  private static JsonSerializerSettings CreateEntitySettings(
    IEntityMaterializer entityMaterializer)
  {
    var settings = CreateBaseSettings();
    settings.Converters = [new CacheEntityCreationConverter(entityMaterializer)];
    return settings;
  }

  private static JsonSerializerSettings CreateMessageSettings(
    IMessageTypeBinder messageTypeBinder)
  {
    var settings = CreateBaseSettings();
    settings.TypeNameHandling = TypeNameHandling.Auto;
    settings.SerializationBinder = messageTypeBinder;
    return settings;
  }

  private static string Serialize(
    object? value,
    Type declaredType,
    JsonSerializerSettings settings)
  {
    var serializer = JsonSerializer.Create(settings);
    using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
    using var jsonWriter = new JsonTextWriter(stringWriter);
    serializer.Serialize(jsonWriter, value, declaredType);
    return stringWriter.ToString();
  }
}
