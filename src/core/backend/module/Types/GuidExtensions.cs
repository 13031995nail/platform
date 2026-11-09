namespace Tmobis.Core.Types;

/// <summary>
/// Вспомогательные методы для работы с Guid.
/// </summary>
public static class GuidExtensions
{
    public static string? ToStringTrim(this Guid? guid)
    => guid == null ? null : ToStringTrim(guid.Value);

    public static string ToStringTrim(this Guid guid)
    => guid.ToString().Trim('{', '}');

    /// <summary>
    /// Перевести Guid в строку и обрезать символы '{' и '}'.
    /// Метод не-расширения для совместимости с Flexberry __PrimaryKey.
    /// </summary>
    /// <param name="guid">GUID.</param>
    /// <returns>Строковое представление GUID без '{' и '}'.</returns>
    [Obsolete("Станет неактуальным при добавлении GUID ID в IDataObject.")]
    public static string? ToStringTrim(object? guid)
    => guid?.ToString()?.Trim('{', '}');
}