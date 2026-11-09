namespace Tmobis.Core.Entities;

public interface IEntityView
{
    string? ViewName { get; init; }

    IReadOnlySet<string> References { get; init; }

    IReadOnlySet<string> SearchProperties { get; init; }
}
