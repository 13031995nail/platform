namespace Tmobis.Reports.Models;

using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

/// <summary>
/// Параметры запуска формирования массового отчета.
/// </summary>
public record GetMassReportRequest : Message
{
  /// <summary>
  /// Набор параметров запуска отчета.
  /// </summary>
  public required List<ReportRunParams> ReportParams { get; init; }

  /// <summary>
  /// Формат файла.
  /// </summary>
  public ReportFormat? Format { get; init; } = ReportFormat.Pdf;

  /// <summary>
  /// ID для поиска подвида ПФ филиала на диске.
  /// </summary>
  public string? FilialId { get; init; }
}
