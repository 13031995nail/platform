namespace Tmobis.Flexberry.Autoincrement.Objects;
using ICSSoft.STORMNET.Business;

/// <summary>
/// Интерфейс для объектов, использующих счетчики автоинкремента.
/// </summary>
[BusinessServer(
    "Tmobis.Flexberry.Autoincrement.BusinessServers.AutoincrementedBS, " +
    "Tmobis.Flexberry.Autoincrement",
    DataServiceObjectEvents.OnAllEvents)]
public interface IAutoincremented
{
    /// <summary>
    /// Ключ счетчика автоинкремента.
    /// </summary>
    string CounterKey { get; }

    /// <summary>
    /// Человекочитаемое имя счетчика автоинкремента.
    /// </summary>
    string CounterName { get; }
    
    /// <summary>
    /// Режим автоинкремента.
    /// </summary>
    AutoincrementMode AutoincrementMode { get; }
    
    void SetAutoincrementedFieldValue(int value);
}
