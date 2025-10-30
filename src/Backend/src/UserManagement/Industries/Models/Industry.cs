using BuildingBlocks.Core.Model;
using UserManagement.Companies.Models;

namespace UserManagement.Industries.Models;

public record Industry : Entity<Guid>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public ICollection<Company> Companies { get; init; } = new List<Company>();
}