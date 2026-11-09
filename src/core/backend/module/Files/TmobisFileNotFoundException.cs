namespace Tmobis.Files;

/// <summary>
/// Исключение для ситуации, когда запись файла или физический объект файла не найдены.
/// </summary>
public class TmobisFileNotFoundException : Exception
{
  /// <summary>
  /// Создаёт исключение без дополнительного сообщения.
  /// </summary>
  public TmobisFileNotFoundException()
  {
  }

  /// <summary>
  /// Создаёт исключение с сообщением об отсутствующем файле.
  /// </summary>
  public TmobisFileNotFoundException(string? message)
    : base(message)
  {
  }

  /// <summary>
  /// Создаёт исключение с сообщением и исходной причиной ошибки.
  /// </summary>
  public TmobisFileNotFoundException(
    string? message,
    Exception? innerException)
    : base(message, innerException)
  {
  }
}
