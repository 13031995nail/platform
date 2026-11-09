namespace Tmobis.Offline.Flexberry.Messages;
using System;
using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

/// <summary>
/// Запрос проверки обновлений, доступных для офлайновых сущностей.
/// </summary>
[MessageMetadata(
    Group = "Offline")]
public record GetOfflineEntitiesUpdateInfo
: DomainCommand<OfflineEntitiesUpdateInfo>
{
    public DateTime? DateStart { get; init; }

    public List<UpdatingObject> Dicts { get; init; } = [];
}

public record UpdatingObject
{
    public string ModelName { get; init; } = string.Empty;

    public DateTime? DateStart { get; init; }

    public DateTime? EditDateStart { get; init; }

    public bool IsFirstLoadCompleted { get; init; }

    public string Limit { get; init; } = string.Empty;

    public List<string> MastersForCheck { get; init; } = [];
}