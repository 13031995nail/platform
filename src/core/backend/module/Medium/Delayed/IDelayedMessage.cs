namespace Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Metas;

public interface IDelayedMessage : IWrappedMessage
{
    DateTime Time { get; set; }
}
