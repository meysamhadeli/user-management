using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Industries.Exceptions;
using UserManagement.Industries.Models;

namespace UserManagement.Industries.Features.CreatingIndustry.V1;

public record CreateIndustryCommand(string Name, string Description) : ICommand<CreateIndustryResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record CreateIndustryResult(Guid Id);

public class CreateIndustryValidator : AbstractValidator<CreateIndustryCommand>
{
    public CreateIndustryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Industry name is required")
            .MaximumLength(100)
            .WithMessage("Industry name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}

public class CreateIndustryHandler : ICommandHandler<CreateIndustryCommand, CreateIndustryResult>
{
    private readonly UserManagementDbContext _dbContext;

    public CreateIndustryHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateIndustryResult> Handle(CreateIndustryCommand command, CancellationToken cancellationToken)
    {
        // Check if industry name already exists
        var industryExists = await _dbContext.Industries.AnyAsync(x => x.Name == command.Name, cancellationToken);

        if (industryExists)
        {
            throw new IndustryAlreadyExistsException(command.Name);
        }

        var industry = command.ToModel(command.Id);

        await _dbContext.Industries.AddAsync(industry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateIndustryResult(industry.Id);
    }
}