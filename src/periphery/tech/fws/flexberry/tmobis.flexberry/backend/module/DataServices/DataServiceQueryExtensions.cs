namespace Tmobis.Frameworks.Flexberry.DataServices;
using ICSSoft.STORMNET;

public static class DataServiceQueryExtensions
{
    public static T? GetByPk<T>(this IQueryable<T> query, Guid pk)
    where T : DataObject
    {
        var pkStr = pk.ToString();
        return query.FirstOrDefault(o => o.__PrimaryKey.Equals(pkStr));
    }
}
