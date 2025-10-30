namespace UserManagement.Users.Dtos;

public record UserDto(Guid Id, string FirstName, string LastName, string UserName, string? Email, Guid CompanyId, string CompanyName);