using BuildingBlocks.Core.Model;
using UserManagement.Companies.Models;

namespace UserManagement.Users.Models;

public record User : Entity<Guid>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string UserName { get; init; }
    public required string PasswordHash { get; init; }
    public string? Email { get; init; }
    public required Guid CompanyId { get; init; }
    public Company Company { get; init; }
}