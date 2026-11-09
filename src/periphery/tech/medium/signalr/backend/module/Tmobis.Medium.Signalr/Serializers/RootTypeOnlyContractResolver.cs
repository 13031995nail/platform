namespace Tmobis.Medium.Signalr.Serializers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

public class RootTypeOnlyContractResolver : DefaultContractResolver
{
  private readonly bool _includeTypeForRoot;

  public RootTypeOnlyContractResolver(bool includeTypeForRoot)
  {
    _includeTypeForRoot = includeTypeForRoot;
  }

  protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
  {
    var property = base.CreateProperty(member, memberSerialization);

    // Отключаем автоматическое добавление $type для всех свойств-объектов
    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
    {
      property.ItemTypeNameHandling = TypeNameHandling.None;
      property.TypeNameHandling = TypeNameHandling.None;
    }

    return property;
  }

  public static void ConfigureSerializer(JsonSerializerSettings settings, bool includeTypeForRoot)
  {
    settings.TypeNameHandling = includeTypeForRoot ? TypeNameHandling.Objects : TypeNameHandling.None;
    settings.ContractResolver = new RootTypeOnlyContractResolver(includeTypeForRoot);
  }
}
