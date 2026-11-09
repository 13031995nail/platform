namespace Tmobis.Offline.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Tmobis.Core.Converters;

/// <summary>
/// Резолвер свойств, которые нужно сериализовать в JSON офлайн бэкап.
/// </summary>
/// <param name="masterType">Тип основного сериализуемого объекта.</param>
public class OfflineJsonContractResolver(
    Type masterType, IPropertyNameConverter propertyNameConverter)
: DefaultContractResolver
{
    /// <summary>
    /// Исключаемые из сериализации свойства.
    /// </summary>
    protected virtual List<string> ExcludedProperties { get; }
    = [];

    private readonly Type masterType = masterType;
    private readonly IPropertyNameConverter propertyNameConverter
    = propertyNameConverter;

  /// <inheritdoc/>
  protected override JsonProperty CreateProperty(
          MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization)
            ?? throw new InvalidOperationException(
                "Ошибка при получении свойства");

        property.PropertyName = propertyNameConverter
            .GetFrontendPropertyName(property?.PropertyName);

        return property;
    }

  /// <inheritdoc/>
  protected override IList<JsonProperty> CreateProperties(
      Type type, MemberSerialization memberSerialization)
  {
    List<JsonProperty> result = [];
    var props = base.CreateProperties(type, memberSerialization);
    foreach (var property in props
        .Where(x => x.PropertyName != null
            && !ExcludedProperties.Contains(x.PropertyName)))
    {
      var propertyName = propertyNameConverter
          .GetBackendPropertyName(property?.PropertyName);
      var isHierarchyProp = !type?
                .GetProperty(propertyName)?
                .DeclaringType?
                .IsAssignableTo(type) ?? false;
      var isMasterType = type == masterType;
      var isIdProp = property?.PropertyName == "id";

      if (isMasterType || isIdProp)
      {
        result.Add(property);
      }
    }

    return result;
  }
}
