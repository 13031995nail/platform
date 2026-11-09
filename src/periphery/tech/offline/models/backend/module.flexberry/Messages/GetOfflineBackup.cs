namespace Tmobis.Offline.Flexberry.Messages;
using Tmobis.Core.Medium.Models;

[MessageMetadata(
    Group = "Offline")]
public record GetOfflineBackup
: DomainCommand<OfflineBackupData>
{
    public string ModelName { get; init; } = string.Empty;

    public Guid? TownId { get; init; }

    public bool LimitByTown { get; init; }

    public Guid? NetDistrictId { get; init; }

    public int? Part { get; init; }
}