namespace Tmobis.Cache.Redis;

using Newtonsoft.Json;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

internal sealed class CacheEntityCreationConverter(
  IEntityMaterializer materializer) : JsonConverter
{
  public override bool CanWrite => false;

  public override bool CanRead => true;

  public override bool CanConvert(Type objectType)
    => typeof(IEntity).IsAssignableFrom(objectType) && objectType.IsInterface;

  public override object? ReadJson(
    JsonReader reader,
    Type objectType,
    object? existingValue,
    JsonSerializer serializer)
    => serializer.Deserialize(
      reader,
      materializer.ResolveBusinessEntityType(objectType));

  public override void WriteJson(
    JsonWriter writer,
    object? value,
    JsonSerializer serializer)
    => throw new NotImplementedException();
}
