namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;

/// <summary>
/// Статус готовности лица.
/// </summary>
public enum PartnerReadinessLevel
{
    /// <summary>
    /// Пусто
    /// </summary>
    [Caption("~")]
    Empty,

    /// <summary>
    /// Готов
    /// </summary>
    [Caption("Готов")]
    Ready,

    /// <summary>
    /// Готов с условиями
    /// </summary>
    [Caption("Готов с условиями")]
    ConditionReady,

    /// <summary>
    /// Не готов
    /// </summary>
    [Caption("Не готов")]
    NotReady,
}
