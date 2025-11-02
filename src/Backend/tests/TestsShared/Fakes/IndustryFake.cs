using Bogus;
using UserManagement.Industries.Models;

namespace IntegrationTests.Fakes;

/// <summary>
/// Creates a fake `Industry` with valid data.
/// </summary>
public sealed class IndustryFake : Faker<Industry>
{
    public IndustryFake()
    {
        CustomInstantiator(f =>
        {
            return new Industry
            {
                Id = Guid.NewGuid(),
                Name = f.Commerce.Department(),
                Description = f.Lorem.Sentence(),
                CreatedAt = f.Date.Past(),
                CreatedBy = f.Random.Long(1, 1000),
                LastModified = f.Date.Recent(),
                LastModifiedBy = f.Random.Long(1, 1000),
                IsDeleted = false,
                Version = 1,
            };
        });
    }

    public IndustryFake WithName(string name)
    {
        RuleFor(x => x.Name, name);
        return this;
    }

    public IndustryFake WithDescription(string description)
    {
        RuleFor(x => x.Description, description);
        return this;
    }

    public IndustryFake AsDeleted()
    {
        RuleFor(x => x.IsDeleted, true);
        return this;
    }
}
