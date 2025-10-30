using UserManagement.Companies.Dtos;
using UserManagement.Companies.Features.CreatingCompany.V1;
using UserManagement.Companies.Models;

namespace UserManagement.Companies.Features;

public static class CompanyMappings
{
    public static CompanyDto ToDto(this Company company)
    {
        ArgumentNullException.ThrowIfNull(company);

        return new CompanyDto(company.Id, company.Name, company.IndustryId, company.Users.Count);
    }

    public static List<CompanyDto> ToDto(this IEnumerable<Company> companies)
    {
        return companies.Select(ToDto).ToList();
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
}
