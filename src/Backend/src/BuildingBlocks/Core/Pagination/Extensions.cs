using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Pagination;

public static class Extensions
{
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
    {
        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PageList<TEntity>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, totalCount);
    }
}
