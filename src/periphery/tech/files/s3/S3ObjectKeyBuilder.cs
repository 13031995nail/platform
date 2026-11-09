namespace Tmobis.Files.Adapter.S3;

internal static class S3ObjectKeyBuilder
{
  public static string Build(Guid fileId, string fileName, string keyPrefix)
  {
    var safeFileName = SanitizeFileName(fileName);
    var objectKey = $"{fileId:D}/{safeFileName}";

    return string.IsNullOrWhiteSpace(keyPrefix)
      ? objectKey
      : $"{keyPrefix}/{objectKey}";
  }

  private static string SanitizeFileName(string fileName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

    var normalized = fileName.Replace('\\', '/');
    var leafName = normalized[(normalized.LastIndexOf('/') + 1)..].Trim();

    if (string.IsNullOrWhiteSpace(leafName))
    {
      throw new ArgumentException(
        "Имя файла после нормализации оказалось пустым.",
        nameof(fileName));
    }

    var safeChars = leafName
      .Where(ch => !char.IsControl(ch))
      .ToArray();
    var safeName = new string(safeChars).Trim();

    if (string.IsNullOrWhiteSpace(safeName))
    {
      throw new ArgumentException(
        "Имя файла содержит только недопустимые символы.",
        nameof(fileName));
    }

    return safeName;
  }
}
