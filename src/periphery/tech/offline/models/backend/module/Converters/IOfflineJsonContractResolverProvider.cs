namespace Tmobis.Offline.Converters;

/// <summary>
/// Поставщик резолвера свойств для сериализации офлайна.
/// </summary>
public interface IOfflineJsonContractResolverProvider
{
    /// <summary>
    /// Получить резолвер свойств для сериализации офлайна.
    /// </summary>
    /// <returns>Резолвер свойств.</returns>
    OfflineJsonContractResolver GetResolver(Type masterType);
}
