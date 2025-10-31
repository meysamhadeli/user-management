using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Users.Features.CheckingEmailAvailability.V1;

public record CheckEmailAvailabilityRequestDto(string Email);

public record CheckEmailAvailabilityResponseDto(bool IsAvailable);

public record CheckEmailAvailabilityCommand(string Email) : IQuery<CheckEmailAvailabilityResult>;

public record CheckEmailAvailabilityResult(bool IsAvailable);

public class CheckEmailAvailabilityCommandValidator : AbstractValidator<CheckEmailAvailabilityCommand>
{
    public CheckEmailAvailabilityCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

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

public class CheckEmailAvailabilityHandler : IQueryHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
{
    private readonly UserManagementDbContext _dbContext;

    public CheckEmailAvailabilityHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckEmailAvailabilityResult> Handle(
        CheckEmailAvailabilityCommand command,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Email);

        var emailExists = await _dbContext
            .Users.AsNoTracking()
            .AnyAsync(x => x.Email == command.Email, cancellationToken);

        return new CheckEmailAvailabilityResult(emailExists);
    }
}
