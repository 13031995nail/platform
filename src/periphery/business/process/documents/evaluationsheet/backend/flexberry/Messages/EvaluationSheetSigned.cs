namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;

public record EvaluationSheetSigned : Message<IEntityUpdatedResult>
{
    public EvaluationSheet Act { get; init; }
}
