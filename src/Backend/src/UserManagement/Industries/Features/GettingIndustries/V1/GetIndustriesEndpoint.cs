using BuildingBlocks.Core.Pagination;
using BuildingBlocks.Web;
using MediatR;
using UserManagement.Industries.Dtos;

namespace UserManagement.Industries.Features.GettingIndustries.V1;

public record GetIndustriesRequestDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Filters = null,
    string? SortOrder = null
) : IPageRequest;

public class GetIndustriesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/industry",
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