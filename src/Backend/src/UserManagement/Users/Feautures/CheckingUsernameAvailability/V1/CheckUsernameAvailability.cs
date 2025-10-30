using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;

public record CheckUsernameAvailabilityRequestDto(string Username);

public record CheckUsernameAvailabilityResponseDto(bool IsAvailable);

public record CheckUsernameAvailabilityCommand(string Username) : IQuery<CheckUsernameAvailabilityResult>;

public record CheckUsernameAvailabilityResult(bool IsAvailable);

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
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class CheckUsernameAvailabilityHandler
    : IQueryHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
{
    private readonly UserManagementDbContext _dbContext;

    public CheckUsernameAvailabilityHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckUsernameAvailabilityResult> Handle(
        CheckUsernameAvailabilityCommand command,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Username);

        if (command.Username.Length < 3)
        {
            return new CheckUsernameAvailabilityResult(false);
        }

        var usernameExists = await _dbContext
            .Users.AsNoTracking()
            .AnyAsync(x => x.UserName == command.Username, cancellationToken);

        return new CheckUsernameAvailabilityResult(!usernameExists);
    }
}
