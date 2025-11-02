using BuildingBlocks.Web;
using MediatR;
using UserManagement.Companies.Dtos;

namespace UserManagement.Companies.Features.GetCompanyById.V1;

public class GetCompanyByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/company/{{id:guid}}",
                async (
                    Guid id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new GetCompanyByIdQuery(id);
                    var result = await mediator.Send(query, cancellationToken);
                    return Results.Ok(result.Company);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("GetCompanyById")
            .Produces<CompanyDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}