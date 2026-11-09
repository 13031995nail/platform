namespace Tmobis.Core.Entities;

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tmobis.Core.Models;

public class IgnoreEntityLinksResolver
: DefaultContractResolver
{
    protected override JsonProperty CreateProperty(
        MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (property.PropertyType?.IsAssignableTo(typeof(IEntity)) ?? false)
        {
            property.ShouldDeserialize = property.ShouldSerialize
            = instance => false;
        }

        return property;
    }
}

public class EntityCreationConverter(
    IEntityMaterializer materializer)
: JsonConverter
{
    public override bool CanWrite => false;
    public override bool CanRead => true;
    
    public override bool CanConvert(Type objectType)
    => objectType.IsAssignableTo(typeof(IEntity)) && objectType.IsInterface;

    public override object? ReadJson(
        JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => serializer.Deserialize(reader, materializer.ResolveBusinessEntityType(objectType));

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}