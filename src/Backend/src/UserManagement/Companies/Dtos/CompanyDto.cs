using UserManagement.Industries.Dtos;

namespace UserManagement.Companies.Dtos;

public record CompanyDto(Guid Id, string Name, IndustryDto? Industry);