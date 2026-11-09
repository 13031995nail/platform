namespace Tmobis.Core.Medium.Models;

public record StatusMessage : Message, IStatusMessageResult
{
    /// <summary>
    /// Была ли ошибка при обработке.
    /// </summary>
    public bool Error { get; init; }
}
