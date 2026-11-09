namespace Tmobis.Offline.Flexberry.Converters;
using Tmobis.Offline.Converters;
using System;
using System.Collections.Generic;
using Tmobis.Core.Converters;

/// <inheritdoc/>
public class FlexberryOfflineJsonContractResolver(
    Type masterType, IPropertyNameConverter propertyNameConverter)
: OfflineJsonContractResolver(masterType, propertyNameConverter)
{
    /// <summary>
    /// Исключаемые из сериализации свойства.
    /// Перечислены свойства DataObject для совместимости с Flexberry.
    /// </summary>
    protected override List<string> ExcludedProperties { get; } =
    ["dynamicProperties", "__PrototypeKey", "prototyped", "__PrimaryKey"];
}
