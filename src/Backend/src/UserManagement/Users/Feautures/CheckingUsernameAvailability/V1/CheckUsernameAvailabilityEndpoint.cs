using BuildingBlocks.Web;
using MediatR;

namespace UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;

public record CheckUsernameAvailabilityRequestDto(string Username);
public record CheckUsernameAvailabilityResponseDto(bool IsAvailable);

public class CheckUsernameAvailabilityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/user/check-availability",
                async (
                    [AsParameters] CheckUsernameAvailabilityRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                                     new CheckUsernameAvailabilityCommand(request.Username),
                                     cancellationToken
                                 );
                    var response = new CheckUsernameAvailabilityResponseDto(result.IsAvailable);
                    return Results.Ok(response);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .WithName("CheckUsernameAvailability")
            .Produces<CheckUsernameAvailabilityResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}