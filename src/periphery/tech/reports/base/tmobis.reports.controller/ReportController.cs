namespace Tmobis.Reports.Controller;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tmobis.Reports.Adapter.UseCases;
using Tmobis.Reports.Models;


/// <summary>
/// Контроллер для формирования и выдачи печатных форм (ПФ) и сопутствующих файлов.
/// Базовый маршрут: <c>api/report</c>.
/// </summary>
[Authorize]
[ApiController]
[Route("api/report")]
public class ReportController(
    IReportUseCase reportUseCase,
    ILogger<ReportController> logger)
: ControllerBase
{
  // Провайдер для определения MIME-типа по расширению файла (используется в GetFile/GetMassPdf).
  private readonly FileExtensionContentTypeProvider contentTypeProvider = new();

  /// <summary>
  /// Формирует отчёт в формате HTML и возвращает его как строку.
  /// </summary>
  /// <param name="runParams">Параметры запуска отчёта (шаблон, данные, формат и т.п.).</param>
  /// <param name="subTypeId">Необязательный подтип ПФ (вариант шаблона).</param>
  /// <returns>HTML-разметка отчёта в кодировке UTF-8, либо текст ошибки.</returns>
  [Route("GetHtml")]
  [HttpPost]
  public async Task<IActionResult> GetHtml(
      ReportRunParams runParams,
      [FromQuery] string? subTypeId)
  {
    // Быстрое исключение, если тело запроса не передано.
    ArgumentNullException.ThrowIfNull(runParams);

    try
    {
      // Принудительно выставляем формат "html" — бэкенд вернёт байты HTML.
      runParams.Format = "html";

      // Запрашиваем у слоя use case сформированный файл (массив байт).
      var report = await reportUseCase.GetFile(runParams, subTypeId);

      // Преобразуем байты в строку UTF-8.
      return Content(Encoding.UTF8.GetString(report), "text/plain", Encoding.UTF8);
    }
    catch (UnauthorizedAccessException)
    {
      return Forbid();
    }
    catch (Exception ex)
    {
      // В ответ пользователю отдаём краткое описание, а детали — в лог.
      logger.LogError(ex, "Ошибка при выгрузке ПФ");
      return Problem(statusCode: StatusCodes.Status500InternalServerError,
          title: "Не удалось сформировать печатную форму");
    }
  }

  /// <summary>
  /// Формирует отчёт в запрошенном формате и отдаёт как файл для скачивания.
  /// </summary>
  /// <param name="runParams">Параметры запуска отчёта (включая желаемый формат).</param>
  /// <param name="subTypeId">Необязательный подтип ПФ.</param>
  /// <returns>Файл-ответ (<see cref="FileContentResult"/>) либо описание ошибки.</returns>
  [Route("GetFile")]
  [HttpPost]
  public async Task<IActionResult> GetFile(
      ReportRunParams runParams,
      [FromQuery] string? subTypeId)
  {
    ArgumentNullException.ThrowIfNull(runParams);

    var isCustomReport = TryExtractCustomReportInfo(runParams, out var customReportName);
    if (isCustomReport)
    {
      var login = User?.Identity?.Name;
      if (string.IsNullOrWhiteSpace(login))
      {
        login = HttpContext?.User?.FindFirst("login")?.Value;
      }
      if (string.IsNullOrWhiteSpace(login))
      {
        login = "unknown";
      }

      var reportName = string.IsNullOrWhiteSpace(customReportName)
          ? runParams.TemplateName
          : customReportName;

      logger.LogError(
          "Экспорт отчета: User={User}; ReportName={ReportName}; Template={TemplateName}; " +
          "Format={Format}; SubType={SubTypeId}",
          login,
          reportName ?? string.Empty,
          runParams.TemplateName ?? string.Empty,
          runParams.Format ?? string.Empty,
          subTypeId ?? string.Empty
      );
    }

    try
    {
      var report = await reportUseCase.GetFile(runParams, subTypeId);
      var extension = reportUseCase.MapFileFormat(runParams.Format);

      var defaultBaseName = string.IsNullOrWhiteSpace(runParams.TemplateName)
          ? $"Report_{DateTime.Now:ddMMyyyy_HHmmss}"
          : $"{runParams.TemplateName}_{DateTime.Now:ddMMyyyy_HHmmss}";

      var fallbackName = EnsureExtension(SanitizeFileName(defaultBaseName), extension);
      var requestedName = EnsureExtension(SanitizeFileName(runParams.FileName), extension);
      var fileNameFull = string.IsNullOrWhiteSpace(requestedName)
          ? fallbackName
          : requestedName;

      // На всякий случай дублируем защиту от пустого имени (не должно случиться).
      if (string.IsNullOrWhiteSpace(fileNameFull))
      {
        fileNameFull = EnsureExtension(
            $"report_{DateTime.Now:ddMMyyyy_HHmmss}",
            extension);
      }

      if (!contentTypeProvider.TryGetContentType(fileNameFull, out string type))
      {
        throw new FormatException("Не удалось установить тип файла.");
      }

      return File(report, type, fileNameFull);
    }
    catch (UnauthorizedAccessException)
    {
      return Forbid();
    }
    catch (FormatException ex)
    {
      logger.LogError(ex, "Ошибка при выгрузке ПФ");
      return Problem(
          detail: ex.Message,
          statusCode: StatusCodes.Status400BadRequest,
          title: "Неверный формат печатной формы");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Ошибка при выгрузке ПФ");
      return Problem(
          detail: ex.Message,
          statusCode: StatusCodes.Status500InternalServerError,
          title: "Не удалось сформировать печатную форму");
    }
  }

  private static bool TryExtractCustomReportInfo(ReportRunParams runParams, out string? reportName)
  {
    reportName = null;
    var isCustom = false;

    if (runParams.Params is Dictionary<string, string> dict)
    {
      if (dict.TryGetValue("__customReport", out var marker)
          && string.Equals(marker, "true", StringComparison.OrdinalIgnoreCase))
      {
        isCustom = true;
      }
      dict.Remove("__customReport");

      if (dict.TryGetValue("__customReportName", out var nameValue))
      {
        reportName = nameValue;
      }
      dict.Remove("__customReportName");
    }

    if (runParams.ParamsList is List<KeyValuePair<string, string>> list)
    {
      var markerIndex = list.FindIndex(kvp => string.Equals(kvp.Key, "__customReport", StringComparison.OrdinalIgnoreCase));
      if (markerIndex >= 0)
      {
        var markerValue = list[markerIndex].Value;
        if (string.Equals(markerValue, "true", StringComparison.OrdinalIgnoreCase))
        {
          isCustom = true;
        }
        list.RemoveAt(markerIndex);
      }

      var nameIndex = list.FindIndex(kvp => string.Equals(kvp.Key, "__customReportName", StringComparison.OrdinalIgnoreCase));
      if (nameIndex >= 0)
      {
        reportName ??= list[nameIndex].Value;
        list.RemoveAt(nameIndex);
      }
    }

    return isCustom;
  }

  /// <summary>
  /// Делает имя файла безопасным для файловой системы:
  /// заменяет недопустимые символы на подчёркивание, удаляет пробелы по краям и точку в конце.
  /// </summary>
  /// <param name="fileName">Исходное имя файла.</param>
  /// <returns>Очищенное имя файла (может быть пустой строкой).</returns>
  private static string SanitizeFileName(string? fileName)
  {
    if (string.IsNullOrWhiteSpace(fileName))
    {
      return string.Empty;
    }

    // Получаем набор недопустимых символов для имён файлов в текущей ОС.
    var invalidChars = Path.GetInvalidFileNameChars();
    var builder = new StringBuilder(fileName.Length);

    // Заменяем каждый недопустимый символ на '_'.
    foreach (var symbol in fileName)
    {
      builder.Append(Array.IndexOf(invalidChars, symbol) >= 0 ? '_' : symbol);
    }

    return builder.ToString().Trim().TrimEnd('.');
  }

  /// <summary>
  /// Гарантирует наличие указанного расширения у имени файла.
  /// Если имя уже содержит требуемое расширение (без учёта регистра), оно не дублируется.
  /// </summary>
  /// <param name="fileName">Имя файла (может быть пустым).</param>
  /// <param name="extension">Расширение без точки или с точкой (напр., <c>"pdf"</c> или <c>".pdf"</c>).</param>
  /// <returns>Имя файла с корректным расширением либо пустая строка.</returns>
  private static string EnsureExtension(string fileName, string extension)
  {
    if (string.IsNullOrWhiteSpace(fileName))
    {
      return string.Empty;
    }

    if (string.IsNullOrWhiteSpace(extension))
    {
      return fileName;
    }

    var normalizedExtension = extension.StartsWith('.')
        ? extension
        : $".{extension}";

    return fileName.EndsWith(normalizedExtension, StringComparison.OrdinalIgnoreCase)
        ? fileName
        : fileName + normalizedExtension;
  }
}
