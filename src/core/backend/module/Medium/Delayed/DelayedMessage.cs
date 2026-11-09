namespace Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Metas;

public record DelayedMessage : WrappedMessage, IDelayedMessage
{
  public DateTime Time { get; set; }
}
