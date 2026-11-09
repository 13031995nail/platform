namespace Tmobis.Offline.Converters;
using Tmobis.Core.Converters;

/// <inheritdoc/>
public class OfflineJsonContractResolverProvider(
    IPropertyNameConverter propertyNameConverter)
: IOfflineJsonContractResolverProvider
{
  private readonly IPropertyNameConverter propertyNameConverter
  = propertyNameConverter;

  /// <inheritdoc/>
  public virtual OfflineJsonContractResolver GetResolver(Type masterType)
    => new(masterType, propertyNameConverter);
}
