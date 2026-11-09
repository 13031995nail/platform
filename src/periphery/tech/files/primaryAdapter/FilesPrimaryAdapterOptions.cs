namespace Tmobis.Files.PrimaryAdapter;

/// <summary>
/// Настройки HTTP-приёма файлов для универсального primary adapter-а.
/// </summary>
public sealed class FilesPrimaryAdapterOptions
{
    /// <summary>
    /// Определение имени секции конфигурации с настройками файлового primary adapter-а.
    /// </summary>
    public const string FilesPrimaryAdapter = "FilesPrimaryAdapter";

    /// <summary>
    /// Максимальный размер загружаемого файла в байтах; null или значение не больше нуля отключают ограничение.
    /// </summary>
    public long? MaxUploadSizeBytes { get; set; }

    /// <summary>
    /// Разрешённые MIME-типы загружаемых файлов; пустой список отключает ограничение.
    /// </summary>
    public string[] AllowedMimeTypes { get; set; } = [];

    /// <summary>
    /// Разрешённые расширения загружаемых файлов; пустой список отключает ограничение.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = [];
}
