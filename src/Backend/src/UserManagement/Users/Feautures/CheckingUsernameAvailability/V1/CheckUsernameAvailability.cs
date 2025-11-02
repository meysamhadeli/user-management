using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;

public record CheckUsernameAvailabilityCommand(string Username) : IQuery<CheckUsernameAvailabilityResult>;

public record CheckUsernameAvailabilityResult(bool IsAvailable);

public class CheckUsernameAvailabilityCommandValidator : AbstractValidator<CheckUsernameAvailabilityCommand>
{
    public CheckUsernameAvailabilityCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_.]+$")
            .WithMessage("Username can only contain letters, numbers, underscores and periods");
    }
}

public class CheckUsernameAvailabilityHandler
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

        var usernameExists = await _dbContext
            .Users.AsNoTracking()
            .AnyAsync(x => x.UserName == command.Username, cancellationToken);

        return new CheckUsernameAvailabilityResult(!usernameExists);
    }
}