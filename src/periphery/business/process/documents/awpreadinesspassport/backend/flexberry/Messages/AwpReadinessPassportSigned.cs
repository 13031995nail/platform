namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;

public record AwpReadinessPassportSigned : Message<IEntityUpdatedResult>
{
    public AwpReadinessPassport Act { get; init; }
}
