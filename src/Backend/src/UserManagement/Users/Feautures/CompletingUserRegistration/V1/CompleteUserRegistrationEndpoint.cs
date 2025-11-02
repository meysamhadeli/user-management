using BuildingBlocks.Web;
using MediatR;

namespace UserManagement.Users.Feautures.CompletingUserRegistration.V1;

public record CompleteUserRegistrationRequestDto(
    Guid CompanyId,
    string FirstName,
    string LastName,
    string UserName,
    string Password,
    string PasswordRepetition,
    string? Email,
    bool AcceptTermsOfService,
    bool AcceptPrivacyPolicy
);

public record CompleteUserRegistrationResponseDto(Guid UserId);

public class CompleteUserRegistrationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(
                $"{EndpointConfig.BaseApiPath}/user/register",
                async (
                    CompleteUserRegistrationRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = request.ToCommand();

                    await mediator.Send(command, cancellationToken);
                    return Results.Created();
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .WithName("CompleteUserRegistration")
            .Produces<CompleteUserRegistrationResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}