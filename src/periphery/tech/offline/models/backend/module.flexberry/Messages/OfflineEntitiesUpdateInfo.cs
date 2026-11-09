namespace Tmobis.Offline.Flexberry.Messages;
using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

/// <summary>
/// Ответ с датой, до которой проверялось наличие обновлений
/// и массивом типов объектов, для которых есть изменения офлайн бэкапов.
/// </summary>
public record OfflineEntitiesUpdateInfo : StatusMessageResult
{
    /// <summary>
    /// Дата в формате ISO 8601.
    /// </summary>
    public string? DateEnd { get; init; }

    /// <summary>
    /// Список измененных типов сущностей.
    /// </summary>
    public List<UpdatedObjectInfo> Types { get; } = [];
}

public record UpdatedObjectInfo
{
    /// <summary>
    /// Тип измененной сущности.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Количество созданных сущностей.
    /// </summary>
    public int? CreatedCount { get; init; }

    /// <summary>
    /// Количество измененных сущностей.
    /// </summary>
    public int? EditedCount { get; init; }
}