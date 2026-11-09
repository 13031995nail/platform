namespace Tmobis.Flexberry.Autoincrement.Objects;

/// <summary>
/// Режим работы автоинкремента.
/// </summary>
public enum AutoincrementMode
{    
    /// <summary>
    /// Ручной режим - ответственность за получение и присвоение значения счетчика лежит на разработчике.
    /// </summary>
    Manual,
    
    /// <summary>
    /// Автоматический режим - нужное поле объекта-наследника <see cref="IAutoincremented"/> инкрементируется бизнес-сервером интерфейса.
    /// </summary>
    Automatic,
}
