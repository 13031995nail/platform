namespace Tmobis.Offline.Flexberry.Converters;

using Tmobis.Core.Converters;
using Tmobis.Offline.Converters;

/// <inheritdoc/>
public class FlexberryOfflineJsonContractResolverProvider(
    IPropertyNameConverter propertyNameConverter)
: IOfflineJsonContractResolverProvider
{
  private readonly IPropertyNameConverter propertyNameConverter
  = propertyNameConverter;

  /// <inheritdoc/>
  public virtual OfflineJsonContractResolver GetResolver(Type masterType)
    => new FlexberryOfflineJsonContractResolver(
        masterType, propertyNameConverter);
}
