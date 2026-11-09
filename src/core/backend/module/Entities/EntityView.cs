namespace Tmobis.Core.Entities;

public record EntityView
: IEntityView
{
    public EntityView(string viewName)
    {
        ViewName = viewName;
        References = new HashSet<string>();
        SearchProperties = new HashSet<string>();
    }

    public EntityView(HashSet<string> references)
    {
        References = references;
        SearchProperties = new HashSet<string>();
    }

    public EntityView(
        string? viewName = null,
        HashSet<string>? references = null,
        HashSet<string>? searchProperties = null)
    {
        References = references ?? [];
        SearchProperties = searchProperties ?? [];
        ViewName = viewName;
    }

    public virtual string? ViewName { get; init; }
    
    public virtual IReadOnlySet<string> References { get; init; }

    public virtual IReadOnlySet<string> SearchProperties { get; init; }
}