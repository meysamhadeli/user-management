using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Users.Feautures.CheckingEmailAvailability.V1;

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

        return new CheckEmailAvailabilityResult(!emailExists);
    }
}
