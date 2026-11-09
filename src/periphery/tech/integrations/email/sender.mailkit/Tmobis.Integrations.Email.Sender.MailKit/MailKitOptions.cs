namespace Tmobis.Integrations.Email.Sender.MailKit;

/// <summary>
/// Настройки почтовго сервиса.
/// </summary>
public class MailKitOptions
{
  /// <summary>
  /// Название блока настроек.
  /// </summary>
  public const string MailKitSender = "MailKitSender";

  /// <summary>
  /// Адрес smtp сервера.
  /// </summary>
  public string Server { get; set; }

  /// <summary>
  /// Порт smtp сервера.
  /// </summary>
  public int Port { get; set; }

  /// <summary>
  /// Настройка ssl.
  /// </summary>
  public bool UseSSL { get; set; }

  /// <summary>
  /// Логин для smtp сервера.
  /// </summary>
  public string Login { get; set; }

  /// <summary>
  /// Пароль для smtp сервера.
  /// </summary>
  public string Password { get; set; }

  /// <summary>
  /// FromName.
  /// </summary>
  public string FromName { get; set; }

  /// <summary>
  /// FromAddress.
  /// </summary>
  public string FromAddress { get; set; }
}
