namespace Tmobis.Frameworks.Web.Backend.Consumers;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Frameworks.Web.Backend.Messages;

public class FrontendErrorConsumer(
    ILogger<FrontendErrorConsumer> logger
) : IMessageConsumer<FrontendError>
{
    /// <inheritdoc/>
    public async Task Consume(FrontendError msg)
    {
        logger.LogError("Ошибка на фронте: {ErrorMessage}. Данные: {@FrontendError}", msg.Message, msg);
    }
}
