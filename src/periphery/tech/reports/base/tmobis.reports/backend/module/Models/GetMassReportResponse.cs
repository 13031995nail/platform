namespace Tmobis.Reports.Models;

using System;
using Tmobis.Core.Medium.Models;

/// <summary>
/// Ответ на запрос массового формирования ПФ.
/// </summary>
public record GetMassReportResponse : StatusMessage
{
  /// <summary>
  /// ID файла.
  /// </summary>
  public Guid? FileId { get; init; }

  /// <summary>
  /// Название файла.
  /// </summary>
  public string? FileName { get; init; }
}
