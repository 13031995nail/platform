namespace Tmobis.Core.Medium.Models;

public interface IStatusMessageResult : IMessageResult
{
    /// <summary>
    /// Была ли ошибка при обработке.
    /// </summary>
    public bool Error { get; init; }
}

public record StatusMessageResult : IStatusMessageResult
{
    /// <summary>
    /// Была ли ошибка при обработке.
    /// </summary>
    public bool Error { get; init; }
}
