using System.Globalization;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Pagination;
using BuildingBlocks.Web;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Companies.Dtos;
using UserManagement.Companies.Models;
using UserManagement.Data;

namespace UserManagement.Companies.Features.GettingCompanies.V1;

public record GetCompaniesRequestDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Filters = null,
    string? SortOrder = null
) : IPageRequest;

public record GetCompaniesQuery(int PageNumber = 1, int PageSize = 20, string? Filters = null, string? SortOrder = null)
    : IPageQuery<GetCompaniesResult>;

public record GetCompaniesResult(IPageList<CompanyDto> Companies);

public class GetCompaniesRequestValidator : AbstractValidator<GetCompaniesRequestDto>
{
    public GetCompaniesRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
    }
}

public class GetCompaniesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/companies",
                async (
                    [AsParameters] GetCompaniesRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = request.ToQuery();

                    var result = await mediator.Send(query, cancellationToken);
                    return Results.Ok(result.Companies);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("GetCompanies")
            .Produces<IPageList<CompanyDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetCompaniesHandler : IQueryHandler<GetCompaniesQuery, GetCompaniesResult>
{
    private readonly UserManagementDbContext _dbContext;

    public GetCompaniesHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCompaniesResult> Handle(GetCompaniesQuery query, CancellationToken cancellationToken)
    {
        var companiesQuery = _dbContext.Companies.AsNoTracking().Include(x => x.Industry).Include(x => x.Users);

        IQueryable<Company> sortedQuery = query.SortOrder?.ToLower(CultureInfo.CurrentCulture) switch
        {
            "name desc" => companiesQuery.OrderByDescending(x => x.Name),
            "name" or "name asc" => companiesQuery.OrderBy(x => x.Name),
            _ => companiesQuery.OrderBy(x => x.Name), // Default sorting
        };

        var companiesPageList = await sortedQuery.ApplyPagingAsync(query, cancellationToken);

        var companyDtos = companiesPageList.Items.ToDto();

        var resultPageList = PageList<CompanyDto>.Create(
            companyDtos.AsReadOnly(),
            companiesPageList.PageNumber,
            companiesPageList.PageSize,
            companiesPageList.TotalCount
        );

        return new GetCompaniesResult(resultPageList);
    }
}
