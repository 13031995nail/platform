namespace Tmobis.Offline.Flexberry.Messages;
using Tmobis.Core.Medium.Models;

public record OfflineBackupData : StatusMessageResult
{
    public bool NotExists { get; init; }

    public string? DateEnd { get; init; }

    public string? Data { get; init; }

    public int? CurPart { get; init; }

    public int? PartsCount { get; init; }
}