namespace Tmobis.Objects.EF;

using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public interface ITestEntity
: IBusinessEntity, IRegisteredEntity
{
    /// <inheritdoc/>
    public string Test { get; set; }

    public List<ITestEntity2> Ents { get; set; }

    public static class Views
    {
        public static readonly EntityView TestView
        = new(nameof(TestView), ["Ents"]);
    }
}
