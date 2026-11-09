namespace Tmobis.Business.Process.Meters.MeterValues.Detection.Consumers;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Assistants.Base.Messages;

public partial class DetectMeterValuesConsumer(
    IMessageDispatcher dispatcher,
    IOptions<MeterValueDetectionOptions> options,
    ILogger<DetectMeterValuesConsumer> logger)
    : IMessageConsumer<DetectMeterValues, DetectedMeterValues>
{
    private readonly IMessageDispatcher dispatcher = dispatcher;
    private readonly ILogger<DetectMeterValuesConsumer> logger = logger;
    private readonly MeterValueDetectionOptions options = options.Value;

    public async Task<DetectedMeterValues> ConsumeWithResult(
        DetectMeterValues message)
    {
        var assistantMessage = new AssistantRequest()
        {
            File = message.File,
            FileMime = message.FileMime,
            FileName = message.FileName,
            HasFile = true,
            Message = options.AssistantQuery
        };

        try
        {
            var result = await dispatcher.SendWithResult<
                AssistantRequest, AssistantResponse>(assistantMessage);
            if (result == null || string.IsNullOrEmpty(result.Message))
            {
                throw new InvalidOperationException(
                    "Не удалось получить ответ распознавания");
            }
            
            if (result.Error)
            {
                throw new InvalidOperationException(result.Message);
            }

            var meterValueStr = MeterValueRegex().Match(result.Message).Value;
            var meterValue = double.Parse(meterValueStr);

            return new DetectedMeterValues()
            {
                Error = false,
                Value = meterValue
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при распознавании показаний");
            return new DetectedMeterValues() { Error = true };
        }
    }

    [GeneratedRegex(@"[\d,.]+")]
    private static partial Regex MeterValueRegex();
}