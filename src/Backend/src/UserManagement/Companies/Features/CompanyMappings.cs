using UserManagement.Companies.Dtos;
using UserManagement.Companies.Features.CreatingCompany.V1;
using UserManagement.Companies.Features.GettingCompanies.V1;
using UserManagement.Companies.Models;
using UserManagement.Industries.Dtos;

namespace UserManagement.Companies.Features;

public static class CompanyMappings
{
    public static CompanyDto ToDto(this Company company)
    {
        var industry = company.Industry != null ?
                           new IndustryDto(company.Industry.Id, company.Industry.Name, company.Industry.Description) :
                           null;

        return new CompanyDto(company.Id, company.Name, industry)
               {
                   Id = company.Id,
                   Name = company.Name,
               };
    }

    public static Company ToModel(this CreateCompanyCommand command, Guid id)
    {
        ArgumentNullException.ThrowIfNull(command);

        return new Company
        {
            Id = id,
            Name = command.Name,
            IndustryId = command.IndustryId,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static List<CompanyDto> ToDto(this IEnumerable<Company> companies)
    {
        return companies.Select(ToDto).ToList();
    }

    public static GetCompaniesQuery ToQuery(this GetCompaniesRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new GetCompaniesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Filters = request.Filters,
            SortOrder = request.SortOrder,
        };
    }
}
