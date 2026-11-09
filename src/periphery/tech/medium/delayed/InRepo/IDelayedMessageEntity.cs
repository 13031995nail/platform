namespace Tmobis.Medium.Delayed.InRepo;
using Tmobis.Medium.InRepo;
public interface IDelayedMessageEntity : IMessageEntity
{
    DateTime Time { get; set; }
}
