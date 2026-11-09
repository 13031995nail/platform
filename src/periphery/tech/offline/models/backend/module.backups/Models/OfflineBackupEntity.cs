namespace Tmobis.Offline.Backups.Models;

public record OfflineBackupEntity
{
    public required string EntityType { get; init; }

    public string? TownPath { get; init; }

    public string? SubPartPath { get; init; }

    public int? PortionSize { get; init; }

    public int? BatchSize { get; init; }
}
