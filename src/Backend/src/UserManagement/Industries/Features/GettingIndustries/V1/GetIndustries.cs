using System.Globalization;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Pagination;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Industries.Dtos;

namespace UserManagement.Industries.Features.GettingIndustries.V1;

public record GetIndustriesRequestDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Filters = null,
    string? SortOrder = null
) : IPageRequest;

public record GetIndustriesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Filters = null,
    string? SortOrder = null
) : IPageQuery<GetIndustriesResult>;

public record GetIndustriesResult(IPageList<IndustryDto> Industries);

public class GetIndustriesRequestValidator : AbstractValidator<GetIndustriesRequestDto>
{
    public GetIndustriesRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
    }
}

public class GetIndustriesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/industries",
                async (
                    [AsParameters] GetIndustriesRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = request.ToQuery();

                    var result = await mediator.Send(query, cancellationToken);
                    return Results.Ok(result.Industries);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Industry").Build())
            .WithName("GetIndustries")
            .Produces<IPageList<IndustryDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetIndustriesHandler : IQueryHandler<GetIndustriesQuery, GetIndustriesResult>
{
    private readonly UserManagementDbContext _dbContext;

    public GetIndustriesHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetIndustriesResult> Handle(GetIndustriesQuery query, CancellationToken cancellationToken)
    {
        var industriesQuery = _dbContext.Industries.AsNoTracking();

        industriesQuery = query.SortOrder?.ToLower(CultureInfo.CurrentCulture) switch
        {
            "name desc" => industriesQuery.OrderByDescending(x => x.Name),
            "name" or "name asc" => industriesQuery.OrderBy(x => x.Name),
            _ => industriesQuery.OrderBy(x => x.Name), // Default sorting
        };

        var industriesPageList = await industriesQuery.ApplyPagingAsync(query, cancellationToken);

        var industryDtos = industriesPageList.Items.ToDto();

        var resultPageList = PageList<IndustryDto>.Create(
            industryDtos.AsReadOnly(),
            industriesPageList.PageNumber,
            industriesPageList.PageSize,
            industriesPageList.TotalCount
        );

        return new GetIndustriesResult(resultPageList);
    }
}
