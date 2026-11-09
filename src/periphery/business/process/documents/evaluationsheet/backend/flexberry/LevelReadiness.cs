namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;

/// <summary>
/// Уровень готовности объекта.
/// </summary>
public enum LevelReadiness
{
    /// <summary>
    /// Не готов.
    /// </summary>
    [Caption("Не готов")]
    NotReady,

    /// <summary>
    /// Готов с условиями.
    /// </summary>
    [Caption("Готов с условиями")]
    ReadyWithConditions,

    /// <summary>
    /// Готов.
    /// </summary>
    [Caption("Готов")]
    Ready,
}
