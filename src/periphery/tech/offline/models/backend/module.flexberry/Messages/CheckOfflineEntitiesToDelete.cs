namespace Tmobis.Offline.Flexberry.Messages;
using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

[MessageMetadata(
    Group = "Offline")]
public record CheckOfflineEntitiesToDelete
: DomainCommand<OfflineEntitiesToDelete>
{
    public List<OfflineEntityToCheckDelete> Entities { get; } = [];
}

public record OfflineEntityToCheckDelete
{
    public string ModelName { get; init; } = string.Empty;

    public List<int> LzwPks { get; } = [];
}