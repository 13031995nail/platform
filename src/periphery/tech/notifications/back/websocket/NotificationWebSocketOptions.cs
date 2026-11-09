namespace Tmobis.Notification.WebSocket;

/// <summary>
/// Настройки WebSocket для оповещений.
/// </summary>
public class NotificationWebSocketOptions
{
    /// <summary>
    /// Название блока настроек.
    /// </summary>
    public const string NotificationWebSocket = "NotificationWebSocket";

    /// <summary>
    /// Ид из справочника INotificationProvider, ассоциируемый с типом WebSocket.
    /// </summary>
    public Guid ProviderId { get; set; }
}
