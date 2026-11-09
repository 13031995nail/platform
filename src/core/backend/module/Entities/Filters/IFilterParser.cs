namespace Tmobis.Core.Entities.Filters;

public interface IFilterParser
{
    public EntityQueryFilter Parse(string[] filter);
}
