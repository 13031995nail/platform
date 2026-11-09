namespace Tmobis.Files.Uploader.Local.Options;

/// <summary>
/// Настройки сервиса загрузки файлов.
/// </summary>
public class FileUploadOptions
{
  /// <summary>
  /// Название блока настроек.
  /// </summary>
  public const string FileUpload = "FileUpload";

  /// <summary>
  /// Gets or sets путь загрузки файлов.
  /// </summary>
  public string UploadPath { get; set; }
}
