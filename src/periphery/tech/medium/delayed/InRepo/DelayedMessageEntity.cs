namespace Tmobis.Medium.Delayed.InRepo;

using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Medium.InRepo;

public class DelayedMessageEntity : FlexberryEntity, IDelayedMessageEntity, IStoredMessageEntity
{
  public DateTime Time { get; set; }

  public string RawMessage { get; set; }
}
