namespace Tmobis.Business.Process.Documents.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Business.Process.Documents.Providers;

[ApiController]
[Authorize]
[Route("api/WorkObjectProcess")]
public sealed class WorkObjectProcessController(
    IWorkObjectProcessProvider provider,
    EvaluationSheetReviewService reviewService,
    WorkObjectRemarksProvider remarksProvider,
    ILogger<WorkObjectProcessController> logger) : ControllerBase
{
    [HttpGet("remarks")]
    public async Task<ActionResult<IReadOnlyList<WorkObjectRemarkGroup>>> GetRemarks(
        [FromQuery] WorkspaceDashboardQuery query)
    {
        try
        {
            return Ok(await remarksProvider.GetRemarks(query));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Не удалось получить список замечаний.");
            return Problem("Не удалось получить список замечаний.");
        }
    }

    [HttpPost("{workObjectId:guid}/review")]
    public async Task<ActionResult> Review(Guid workObjectId,
        [FromQuery] Guid? awpId, [FromQuery] string operation)
    {
        try
        {
            EvaluationSheetReviewService.ValidateOperation(operation);
            var preparation = await provider.GetPreparation(workObjectId, awpId);
            if (!preparation.EvaluationSheetId.HasValue)
            {
                return BadRequest("Сначала создайте оценочный лист.");
            }
            await reviewService.Review(preparation.EvaluationSheetId.Value, operation);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Не удалось выполнить действие проверки объекта.");
            return Problem("Не удалось выполнить действие проверки объекта.");
        }
    }

    [HttpGet("{workObjectId:guid}")]
    public async Task<ActionResult<WorkObjectProcess>> GetProcess(Guid workObjectId, [FromQuery] Guid? awpId)
    {
        try
        {
            return Ok(await provider.GetProcess(workObjectId, awpId));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Не удалось получить карточку объекта.");
            return Problem("Не удалось получить данные объекта.");
        }
    }
}
