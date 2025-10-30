using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Industries.Dtos;

namespace UserManagement.Industries.Features.GettingIndustries.V1;

public record GetIndustriesQuery : IQuery<GetIndustriesResult>;

public record GetIndustriesResult(IEnumerable<IndustryDto> Industries);

public class GetIndustriesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                $"{EndpointConfig.BaseApiPath}/industry",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetIndustriesQuery(), cancellationToken);
                    return Results.Ok(result.Industries);
                }
            )
            .WithApiVersionSet(builder.NewApiVersionSet("Industry").Build())
            .WithName("GetIndustries")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetIndustriesHandler : IQueryHandler<GetIndustriesQuery, GetIndustriesResult>
{
    private readonly UserManagementDbContext _dbContext;

    public GetIndustriesHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetIndustriesResult> Handle(GetIndustriesQuery query, CancellationToken cancellationToken)
    {
        var industries = await _dbContext.Industries.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

        return new GetIndustriesResult(industries.ToDto());
    }
}
