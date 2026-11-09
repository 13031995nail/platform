namespace Tmobis.Objects.EF;

using Tmobis.Core.Models;

public interface ITestEntity2
: IBusinessEntity, IRegisteredEntity
{
    /// <inheritdoc/>
    public string Test2 { get; set; }

    public Guid EntityRefId { get; set; }

    public ITestEntity EntityRef { get; set; }
}
