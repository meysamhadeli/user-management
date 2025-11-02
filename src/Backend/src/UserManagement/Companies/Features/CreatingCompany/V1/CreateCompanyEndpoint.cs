using BuildingBlocks.Web;
using MediatR;

namespace UserManagement.Companies.Features.CreatingCompany.V1;

public record CreateCompanyRequestDto(string Name, Guid IndustryId);

public record CreateCompanyResponseDto(Guid Id);

public class CreateCompanyEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(
                $"{EndpointConfig.BaseApiPath}/company",
                async (CreateCompanyRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new CreateCompanyCommand(request.Name, request.IndustryId);
                    await mediator.Send(command, cancellationToken);
                    return Results.Created();
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("CreateCompany")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}