namespace Tmobis.Files.PrimaryAdapter;

/// <summary>
/// Проверка параметров запроса загрузки файла.
/// </summary>
internal sealed class FileUploadRequestValidator
{
    /// <summary>
    /// Проверка имени файла на отсутствие пути, недопустимых символов и пустого значения.
    /// </summary>
    public bool IsFileNameValid(string? fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName)
            && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
            && !fileName.Contains('/')
            && !fileName.Contains('\\')
            && Path.GetFileName(fileName) == fileName;
    }

    /// <summary>
    /// Проверка расширения файла с учётом пустого списка разрешений как отсутствия ограничения.
    /// </summary>
    public bool IsExtensionAllowed(
        string extension,
        string[]? allowedExtensions)
    {
        var normalizedExtensions = NormalizeExtensions(allowedExtensions);
        if (normalizedExtensions.Count == 0)
        {
            return true;
        }

        return normalizedExtensions.Contains(NormalizeExtension(extension));
    }

    /// <summary>
    /// Проверка MIME-типа запроса с учётом параметров Content-Type и пустого списка разрешений.
    /// </summary>
    public bool IsMimeTypeAllowed(
        string? contentType,
        string[]? allowedMimeTypes)
    {
        if (allowedMimeTypes == null || allowedMimeTypes.Length == 0)
        {
            return true;
        }

        var normalizedContentType = NormalizeContentType(contentType);
        if (string.IsNullOrWhiteSpace(normalizedContentType))
        {
            return false;
        }

        return allowedMimeTypes.Any((allowedMimeType) =>
            string.Equals(
                NormalizeContentType(allowedMimeType),
                normalizedContentType,
                StringComparison.OrdinalIgnoreCase));
    }

    private static string? NormalizeContentType(string? contentType)
    {
        return contentType?.Split(';', 2)[0].Trim();
    }

    private static HashSet<string> NormalizeExtensions(string[]? extensions)
    {
        return (extensions ?? [])
            .Where((extension) => !string.IsNullOrWhiteSpace(extension))
            .Select(NormalizeExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeExtension(string extension)
    {
        var normalized = extension.Trim();
        if (string.IsNullOrEmpty(normalized) || normalized.StartsWith('.'))
        {
            return normalized;
        }

        return $".{normalized}";
    }
}
