using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Companies.Dtos;
using UserManagement.Data;

namespace UserManagement.Companies.Features.GettingCompanies.V1;

public record GetCompaniesQuery : IQuery<GetCompaniesResult>;

public record GetCompaniesResult(IEnumerable<CompanyDto> Companies);

public class GetCompaniesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/company",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCompaniesQuery(), cancellationToken);
                    return Results.Ok(result.Companies);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Company").Build())
            .WithName("GetCompanies")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetCompaniesHandler : IQueryHandler<GetCompaniesQuery, GetCompaniesResult>
{
    private readonly UserManagementDbContext _dbContext;

    public GetCompaniesHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCompaniesResult> Handle(GetCompaniesQuery query, CancellationToken cancellationToken)
    {
        var companies = await _dbContext
            .Companies.AsNoTracking()
            .Include(x => x.Industry)
            .Include(x => x.Users)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new GetCompaniesResult(companies.ToDto());
    }
}
