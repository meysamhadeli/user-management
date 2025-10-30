using BuildingBlocks.Core.Model;
using UserManagement.Industries.Models;
using UserManagement.Users.Models;

namespace UserManagement.Companies.Models;

public record Company : Entity<Guid>
{
    public required string Name { get; init; }
    public required Guid IndustryId { get; init; }
    public Industry Industry { get; init; }
    public ICollection<User> Users { get; init; } = new List<User>();
}