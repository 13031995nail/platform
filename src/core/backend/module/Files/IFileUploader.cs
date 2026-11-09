namespace Tmobis.Files.Uploader;

/// <summary>
/// Провайдер загрузки файлов
/// </summary>
public interface IFileUploader
{
  /// <summary>
  /// Загрузка файлов.
  /// </summary>
  /// <param name="fileContent">Данные файла.</param>
  /// <param name="fileName">Название файла.</param>
  /// <returns> Ключ файла.</returns>
  public Task<Guid> UploadFile(byte[] fileContent, string fileName);
}
