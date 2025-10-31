using BuildingBlocks.Core.CQRS;

namespace BuildingBlocks.Core.Pagination;

public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : class { }
