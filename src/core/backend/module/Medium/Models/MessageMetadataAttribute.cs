namespace Tmobis.Core.Medium.Models;

[AttributeUsage(AttributeTargets.Class)]
public class MessageMetadataAttribute : Attribute
{
    public MessageMetadataAttribute()
    {

    }

    public MessageMetadataAttribute(
        string? group, string? name)
    {
        Group = group;
        Name = name;
    }

    public string? Group { get; set; }

    public string? Name { get; set; }
}
