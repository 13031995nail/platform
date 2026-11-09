namespace Tmobis.Core.Entities.Filters;

public record EntityQueryFilterOperation
{
    public required string PropertyName { get; set; }
    public FilterOperation? Operator { get; set; }
    public required object Value { get; set; }
}