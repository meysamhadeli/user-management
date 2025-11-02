using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserManagement.Companies.Dtos;
using UserManagement.Companies.Exceptions;
using UserManagement.Data;

namespace UserManagement.Companies.Features.GetCompanyById.V1;

public record GetCompanyByIdQuery(Guid Id) : IQuery<GetCompanyByIdResult>;
public record GetCompanyByIdResult(CompanyDto Company);

public class GetCompanyByIdValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID is required");
    }
}

public class GetCompanyByIdHandler : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdResult>
{
    private readonly UserManagementDbContext _dbContext;

    public GetCompanyByIdHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCompanyByIdResult> Handle(GetCompanyByIdQuery query, CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .AsNoTracking()
            .Include(x => x.Industry)
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (company is null)
        {
            throw new CompanyNotFoundException(query.Id);
        }

        var companyDto = company.ToDto();

        return new GetCompanyByIdResult(companyDto);
    }
}