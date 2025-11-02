using BuildingBlocks.Core.Pagination;
using BuildingBlocks.Web;
using MediatR;
using UserManagement.Companies.Dtos;

namespace UserManagement.Companies.Features.GettingCompanies.V1;

public record GetCompaniesRequestDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Filters = null,
    string? SortOrder = null
) : IPageRequest;

public class GetCompaniesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/companies",
                async (
                    [AsParameters] GetCompaniesRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = request.ToQuery();

                    var result = await mediator.Send(query, cancellationToken);
                    return Results.Ok(result.Companies);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("GetCompanies")
            .Produces<IPageList<CompanyDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}