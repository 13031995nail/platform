namespace Tmobis.Core.Entities;

[Flags]
public enum EntityVisibility
{
    None = 0,
    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class InternalEntityAttribute(EntityVisibility visibility = EntityVisibility.None)
: Attribute
{
    public virtual EntityVisibility Visibility { get; } = visibility;
}
