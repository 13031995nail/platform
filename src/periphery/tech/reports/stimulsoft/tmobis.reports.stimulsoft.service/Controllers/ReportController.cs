namespace Tmobis.Reports.Stimulsoft.Service.Controllers;

using Auth.ApiKey;
using Microsoft.AspNetCore.Mvc;
using Tmobis.Reports.Stimulsoft.Service.UseCases;
using Tmobis.Reports.Models;

[ApiController]
[ApiKeyAuthorize]
[Route("api/reports")]
public class ReportController(
    IReportExporter reportBuilder,
    ILogger<ReportExporter> logger)
: ControllerBase
{
  [HttpPost]
  [Route("buildReport")]
  public IActionResult BuildReport(GetReportRequest request)
  {
    try
    {
      if (!ValidateRequest(request))
      {
        return ValidationProblem();
      }

      return File(reportBuilder.ExportReport(request), "application/octet-stream");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Ошибка при построении печатной формы");
      return Problem();
    }
  }

  private bool ValidateRequest(GetReportRequest request)
  {
    return request != null
      && !string.IsNullOrEmpty(request.TemplateName)
      && !string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(request.TemplateName))
      && request.Format.HasValue;
  }
}
