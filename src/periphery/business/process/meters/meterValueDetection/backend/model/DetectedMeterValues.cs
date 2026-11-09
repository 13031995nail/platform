namespace Tmobis.Business.Process.Meters.MeterValues.Detection;
using Tmobis.Core.Medium.Models;

public record DetectedMeterValues : StatusMessageResult
{
    public double? Value { get; init; }
}