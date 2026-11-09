namespace Tmobis.Business.Core.TownLimit;

using System.Collections.Concurrent;

/// <inheritdoc/>
public class TownLimitResolver : ITownLimitResolver
{
  private readonly ConcurrentDictionary<Type, TownLimitDefinition> cache = new();

  /// <inheritdoc/>
  public TownLimitDefinition? GetTownLimit(Type type)
  {
    ArgumentNullException.ThrowIfNull(type);
    if (cache.TryGetValue(type, out var cachedTownLimit))
    {
      return cachedTownLimit;
    }

    var townLimit = ResolveTownLimit(type);
    if (townLimit != null)
    {
      cache.TryAdd(type, townLimit);
    }

    return townLimit;
  }

  private static TownLimitDefinition? ResolveTownLimit(Type type)
  {
    var attribute = GetAttribute(type)
      ?? type
        .GetInterfaces()
        .Select(GetAttribute)
        .FirstOrDefault(static attr => attr != null);

    return attribute == null
      ? null
      : new TownLimitDefinition(attribute.TownPropertyPath);
  }

  private static TownLimitedAttribute? GetAttribute(Type type)
    => Attribute.GetCustomAttribute(
      type,
      typeof(TownLimitedAttribute),
      inherit: true) as TownLimitedAttribute;
}
