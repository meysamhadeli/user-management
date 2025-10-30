using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Industries.Exceptions;
using UserManagement.Industries.Models;

namespace UserManagement.Industries.Features.CreatingIndustry.V1;

public record CreateIndustryRequestDto(string Name, string Description);

public record CreateIndustryResponseDto(Guid Id);

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
                    var result = await mediator.Send(command, cancellationToken);
                    var response = new CreateIndustryResponseDto(result.Id);
                    return Results.CreatedAtRoute("GetIndustryById", new { id = result.Id }, response);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Industry").Build())
            .WithName("CreateIndustry")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class CreateIndustryHandler : ICommandHandler<CreateIndustryCommand, CreateIndustryResult>
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
