using UserManagement.Industries.Dtos;
using UserManagement.Industries.Features.CreatingIndustry.V1;
using UserManagement.Industries.Features.GettingIndustries.V1;
using UserManagement.Industries.Models;

namespace UserManagement.Industries.Features;

public static class IndustryMappings
{
    public static IndustryDto ToDto(this Industry industry)
    {
        ArgumentNullException.ThrowIfNull(industry);

        return new IndustryDto(industry.Id, industry.Name, industry.Description);
    }

    public static Industry ToModel(this CreateIndustryCommand command, Guid id)
    {
        ArgumentNullException.ThrowIfNull(command);

        return new Industry
        {
            Id = id,
            Name = command.Name,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static List<IndustryDto> ToDto(this IEnumerable<Industry> industries)
    {
        return industries.Select(ToDto).ToList();
    }

    public static GetIndustriesQuery ToQuery(this GetIndustriesRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new GetIndustriesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Filters = request.Filters,
            SortOrder = request.SortOrder,
        };
    }
}