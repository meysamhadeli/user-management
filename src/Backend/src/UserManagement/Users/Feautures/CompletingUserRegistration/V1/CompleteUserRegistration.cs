using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Users.Exceptions;

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

public record CompleteUserRegistrationCommand(
    Guid CompanyId,
    string FirstName,
    string LastName,
    string UserName,
    string Password,
    string PasswordRepetition,
    string? Email,
    bool AcceptTermsOfService,
    bool AcceptPrivacyPolicy
) : ICommand<CompleteUserRegistrationResult>;

public record CompleteUserRegistrationResult(Guid UserId);

public class CompleteUserRegistrationValidator : AbstractValidator<CompleteUserRegistrationCommand>
{
    public CompleteUserRegistrationValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("Company is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.PasswordRepetition).Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.AcceptTermsOfService).Equal(true).WithMessage("You must accept the terms of service");

        RuleFor(x => x.AcceptPrivacyPolicy).Equal(true).WithMessage("You must accept the privacy policy");
    }
}

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
                    var command = new CompleteUserRegistrationCommand(
                        request.CompanyId,
                        request.FirstName,
                        request.LastName,
                        request.UserName,
                        request.Password,
                        request.PasswordRepetition,
                        request.Email,
                        request.AcceptTermsOfService,
                        request.AcceptPrivacyPolicy
                    );

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

public class CompleteUserRegistrationHandler
    : ICommandHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
{
    private readonly UserManagementDbContext _dbContext;

    public CompleteUserRegistrationHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CompleteUserRegistrationResult> Handle(
        CompleteUserRegistrationCommand command,
        CancellationToken cancellationToken
    )
    {
        if (!command.AcceptTermsOfService || !command.AcceptPrivacyPolicy)
        {
            throw new TermsNotAcceptedException();
        }

        var companyExists = await _dbContext.Companies.AnyAsync(x => x.Id == command.CompanyId, cancellationToken);

        if (!companyExists)
        {
            throw new CompanyNotFoundException(command.CompanyId);
        }

        var usernameExists = await _dbContext.Users.AnyAsync(x => x.UserName == command.UserName, cancellationToken);

        if (usernameExists)
        {
            throw new UsernameAlreadyExistsException(command.UserName);
        }

        var emailExists = await _dbContext.Users.AnyAsync(x => x.Email == command.Email, cancellationToken);

        if (emailExists)
        {
            throw new EmailAlreadyExistsException(command.Email);
        }

        var passwordHash = PasswordHasher.HashPassword(command.Password);
        var user = command.ToModel(Guid.NewGuid(), passwordHash, command.CompanyId);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteUserRegistrationResult(user.Id);
    }
}
