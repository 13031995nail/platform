namespace Tmobis.Offline.Flexberry.Messages;
using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

public record OfflineEntitiesToDelete : StatusMessageResult
{
    public List<OfflineEntityDeleted> Data { get; } = [];
}

public record OfflineEntityDeleted
{
    public string ModelName { get; init; } = string.Empty;

    public List<string> Pks { get; } = [];
}