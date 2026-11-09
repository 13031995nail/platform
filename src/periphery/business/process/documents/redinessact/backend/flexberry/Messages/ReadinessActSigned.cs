namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;

public record ReadinessActSigned : Message<IEntityUpdatedResult>
{
    public RedinessAct Act { get; init; }
}
