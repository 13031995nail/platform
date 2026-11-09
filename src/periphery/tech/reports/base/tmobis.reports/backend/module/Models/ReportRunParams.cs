namespace Tmobis.Reports.Models
{
  using System;
  using System.Collections.Generic;
  using Tmobis.Core.Medium.Models;

  /// <summary>
  /// Параметры запуска отчета.
  /// </summary>
  public class ReportRunParams
  {
    /// <summary>
    /// Имя шаблона.
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// Id файла.
    /// </summary>
    public Guid? PrintingFileId { get; set; }

    /// <summary>
    /// Список параметров.
    /// </summary>
    public Dictionary<string, string>? Params { get; set; }

    /// <summary>
    /// Список параметров.
    /// </summary>
    public List<KeyValuePair<string, string>>? ParamsList { get; set; }

    /// <summary>
    /// Имя файла.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Формат файла.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Формат файла.
    /// </summary>
    public string? FileExt { get; set; }

    /// <summary>
    /// Имя принтера для печати.
    /// </summary>
    public string? PrinterName { get; set; }

    /// <summary>
    /// Число копий.
    /// </summary>
    public short? Copies { get; set; }

    /// <summary>
    /// Цветная печать.
    /// </summary>
    public bool? IsColor { get; set; }

    /// <summary>
    /// Двусторонняя печать.
    /// </summary>
    public bool? IsDoubleSide { get; set; }

    /// <summary>
    /// Ориентация: Книжная/Альбомная.
    /// </summary>
    public string? Orientation { get; set; }

    public string? TownId { get; set; }
  }
}
