using BuildingBlocks.Web;
using MediatR;

namespace UserManagement.Users.Feautures.CheckingEmailAvailability.V1;

public record CheckEmailAvailabilityRequestDto(string Email);

public record CheckEmailAvailabilityResponseDto(bool IsAvailable);

public class CheckEmailAvailabilityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/user/check-email-availability",
                async (
                    [AsParameters] CheckEmailAvailabilityRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = new CheckEmailAvailabilityCommand(request.Email);
                    var result = await mediator.Send(command, cancellationToken);
                    var response = new CheckEmailAvailabilityResponseDto(result.IsAvailable);
                    return Results.Ok(response);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .WithName("CheckEmailAvailability")
            .Produces<CheckEmailAvailabilityResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}