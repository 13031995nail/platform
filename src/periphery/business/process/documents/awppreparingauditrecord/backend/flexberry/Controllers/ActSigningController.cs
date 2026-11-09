namespace Tmobis.Business.Process.Documents.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Authorize]
[Route("api/ActSigning")]
public sealed class ActSigningController(
    ActSigningCancellationService service,
    ILogger<ActSigningController> logger) : ControllerBase
{
    [HttpPost("{signModel}/{documentId:guid}/cancel")]
    public async Task<ActionResult> Cancel(string signModel, Guid documentId)
    {
        try
        {
            await service.Cancel(signModel, documentId);
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
            logger.LogError(exception, "Не удалось отменить подписание документа.");
            return Problem("Не удалось отменить подписание документа.");
        }
    }
}
