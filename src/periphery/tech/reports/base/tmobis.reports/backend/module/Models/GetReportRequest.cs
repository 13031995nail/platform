namespace Tmobis.Reports.Models;

using System.Collections.Generic;
using Tmobis.Core.Medium.Models;

public enum ReportFormat { Html, Pdf, Docx, Xlsx, Csv }

/// <summary>
/// Параметры запуска отчета.
/// </summary>
public record GetReportRequest : Message
{
  /// <summary>
  /// Имя шаблона.
  /// </summary>
  public string? TemplateName { get; init; }

  /// <summary>
  /// Список параметров.
  /// </summary>
  public Dictionary<string, string>? Params { get; init; }

  /// <summary>
  /// Формат файла.
  /// </summary>
  public ReportFormat? Format { get; init; }
  
  /// <summary>
  /// Имя итогового файла.
  /// </summary>
  public string? FileName { get; init; }

  /// <summary>
  /// Постфикс для имени печатной формы на диске.
  /// </summary>
  public string? SubType { get; init; }

  /// <summary>
  /// ID для поиска подвида ПФ филиала на диске.
  /// </summary>
  public string? FilialId { get; init; }
}