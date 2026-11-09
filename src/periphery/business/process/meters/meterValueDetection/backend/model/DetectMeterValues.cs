namespace Tmobis.Business.Process.Meters.MeterValues.Detection;
using Tmobis.Core.Medium.Models;

public record DetectMeterValues : Message<DetectedMeterValues>
{
    public string? FileName { get; init; }

    public string? FileMime { get; init; }

    public byte[]? File { get; init; }
}