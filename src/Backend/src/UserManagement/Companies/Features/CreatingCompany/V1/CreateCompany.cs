using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Companies.Exceptions;
using UserManagement.Companies.Models;
using UserManagement.Data;

namespace UserManagement.Companies.Features.CreatingCompany.V1;

public record CreateCompanyRequestDto(string Name, Guid IndustryId);

public record CreateCompanyResponseDto(Guid Id);

public record CreateCompanyCommand(string Name, Guid IndustryId) : ICommand<CreateCompanyResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record CreateCompanyResult(Guid Id);

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.IndustryId).NotEmpty().WithMessage("Industry is required");
    }
}

public class CreateCompanyEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(
                $"{EndpointConfig.BaseApiPath}/company",
                async (CreateCompanyRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new CreateCompanyCommand(request.Name, request.IndustryId);
                    var result = await mediator.Send(command, cancellationToken);
                    var response = new CreateCompanyResponseDto(result.Id);
                    return Results.CreatedAtRoute("GetCompanyById", new { id = result.Id }, response);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("CreateCompany")
            .Produces<CreateCompanyResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateCompanyHandler : ICommandHandler<CreateCompanyCommand, CreateCompanyResult>
{
    private readonly UserManagementDbContext _dbContext;

    public CreateCompanyHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        // Validate industry exists
        var industryExists = await _dbContext.Industries.AnyAsync(x => x.Id == command.IndustryId, cancellationToken);

        if (!industryExists)
        {
            throw new IndustryNotFoundException(command.IndustryId);
        }

        // Check if company name already exists
        var companyExists = await _dbContext.Companies.AnyAsync(x => x.Name == command.Name, cancellationToken);

        if (companyExists)
        {
            throw new CompanyAlreadyExistsException(command.Name);
        }

        var company = command.ToModel(command.Id);

        await _dbContext.Companies.AddAsync(company, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCompanyResult(company.Id);
    }
}
