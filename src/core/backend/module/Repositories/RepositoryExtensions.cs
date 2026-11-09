namespace Tmobis.Core.Repositories;

using System.Linq.Expressions;

public static class RepositoryExtensions
{
    extension<T>(IQueryable<T> source)
    {
        public IQueryable<T> ApplyListQuery(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null)
        {
            if (predicate != null)
            {
                source = source.Where(predicate);
            }

            if (orderBy != null)
            {
                source = orderBy(source);
            }

            if (skip.HasValue)
            {
                source = source.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                source = source.Take(take.Value);
            }

            return source;
        }
    }
}
