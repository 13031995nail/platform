namespace Tmobis.Core.Entities;

using System.Linq.Expressions;
using Messages;
using Models;
using Repositories;

public static class EntityListQueryHelper
{
    public static async Task<EntityListQueryResult<TEntity>> GetList<TEntity>(
        IReadRepository repository,
        int? page,
        int? perPage,
        int defaultPerPage,
        int maxPerPage,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool includeTotalCount = false,
        Expression<Func<TEntity, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var pagination = CalculatePagination(
            page,
            perPage,
            defaultPerPage,
            maxPerPage);
        var totalCount = includeTotalCount
            ? await repository.Count(predicate, view, cancellationToken)
            : (int?)null;

        var normalizedPage = pagination.Page;
        if (totalCount.HasValue)
        {
            normalizedPage = ClampPageToTotal(
                normalizedPage,
                pagination.Take,
                totalCount.Value);
        }

        var skip = ((long)normalizedPage - 1) * pagination.Take;
        if (skip > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(page), page, "Номер страницы слишком велик.");
        }

        var data = await repository.Find(
            predicate,
            view,
            orderBy,
            (int)skip,
            pagination.Take,
            cancellationToken);

        return new EntityListQueryResult<TEntity>(
            data,
            normalizedPage,
            pagination.Take,
            totalCount);
    }

    private static (int Page, int Take) CalculatePagination(
        int? page,
        int? perPage,
        int defaultPerPage,
        int maxPerPage)
    {
        var normalizedPage = Math.Max(1, page ?? 1);
        var normalizedPerPage = Math.Clamp(
            perPage is null or <= 0 ? defaultPerPage : perPage.Value,
            1,
            maxPerPage);

        return (normalizedPage, normalizedPerPage);
    }

    private static int ClampPageToTotal(
        int page,
        int perPage,
        int totalCount)
    {
        var maxPage = Math.Max(
            1,
            (int)(((long)totalCount + perPage - 1) / perPage));

        return Math.Min(page, maxPage);
    }
}
