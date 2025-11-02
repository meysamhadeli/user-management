using BuildingBlocks.Web;
using MediatR;

namespace UserManagement.Industries.Features.CreatingIndustry.V1;

public record CreateIndustryRequestDto(string Name, string Description);
public record CreateIndustryResponseDto(Guid Id);

public class CreateIndustryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(
                $"{EndpointConfig.BaseApiPath}/industry",
                async (CreateIndustryRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new CreateIndustryCommand(request.Name, request.Description);
                    await mediator.Send(command, cancellationToken);
                    return Results.Created();
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Industry").Build())
            .WithName("CreateIndustry")
            .Produces<CreateIndustryResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}