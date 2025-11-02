using UserManagement.Companies.Models;
using UserManagement.Industries.Models;

namespace UserManagement.Data.Seeds;

public static class InitialData
{
    public static List<Industry> Industries { get; }
    public static List<Company> Companies { get; }

    static InitialData()
    {
        Industries = new List<Industry>
        {
            new Industry{Id = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789001"), Name = "Technology", Description = "Technology and software companies", CreatedAt = DateTime.Now},
            new Industry{Id = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789002"), Name = "Healthcare", Description = "Healthcare and medical services", CreatedAt = DateTime.Now},
            new Industry{Id = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789003"), Name = "Finance", Description = "Banking and financial services", CreatedAt = DateTime.Now},
            new Industry{Id = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789004"), Name = "Manufacturing", Description = "Industrial manufacturing", CreatedAt = DateTime.Now},
            new Industry{Id = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789005"), Name = "Retail", Description = "Retail and e-commerce", CreatedAt = DateTime.Now},
        };

        Companies = new()
        {
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789101"), Name = "Microsoft", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789001"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789102"), Name = "Google", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789001"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789103"), Name = "Apple", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789001"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789104"), Name = "Johnson & Johnson", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789002"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789105"), Name = "JPMorgan Chase", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789003"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789106"), Name = "Amazon", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789005"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789107"), Name = "Tesla", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789004"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789108"), Name = "Pfizer", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789002"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789109"), Name = "Goldman Sachs", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789003"), CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.Parse("b1b2c3d4-1234-5678-9abc-123456789110"), Name = "Walmart", IndustryId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789005"), CreatedAt = DateTime.UtcNow }
        };
    }
}