namespace Tmobis.Files.Provider;

/// <summary>
/// Сервис получения файлов
/// </summary>
public interface IFileProvider
{
    /// <summary>
    /// Получение файла по его идентификатору
    /// (потенциально требует догрузки данных из хранилища).
    /// </summary>
    /// <param name="fileId">ИД файла.</param>
    /// <returns>Содержимое файла.</returns>
    public Task<byte[]> GetFile(Guid fileId);

    /// <summary>
    /// Получение файла по его идентификатору и имени файла.
    /// </summary>
    /// <param name="fileId">ИД файла.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Содержимое файла.</returns>
    public Task<byte[]> GetFile(Guid fileId, string fileName);

    /// <summary>
    /// Получение пути (или удаленного адреса) к файлу.
    /// </summary>
    /// <param name="fileId">ИД файла.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Путь (адрес) файла.</returns>
    public Task<byte[]> GetFilePath(Guid fileId, string fileName);
}
