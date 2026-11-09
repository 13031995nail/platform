namespace Tmobis.Business.Process.Meters.MeterValues.Detection.Consumers;

public class MeterValueDetectionOptions
{
    public const string MeterValueDetection = "MeterValueDetection";

    public string AssistantQuery { get; set; }
    = "Распознать показание прибора учета, ответ число с цифрами до запятой";
}