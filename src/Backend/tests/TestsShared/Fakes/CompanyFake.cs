using Bogus;
using UserManagement.Companies.Models;

namespace IntegrationTests.Fakes;

/// <summary>
/// Creates a fake `Company` with valid data adhering to domain rules.
/// </summary>
public sealed class CompanyFake : Faker<Company>
{
    public CompanyFake(Guid industryId)
    {
        CustomInstantiator(f =>
        {
            return new Company
            {
                Id = Guid.NewGuid(),
                Name = f.Company.CompanyName(),
                IndustryId = industryId,
                CreatedAt = f.Date.Past(),
                CreatedBy = f.Random.Long(1, 1000),
                LastModified = f.Date.Recent(),
                LastModifiedBy = f.Random.Long(1, 1000),
                IsDeleted = false,
                Version = 1,
            };
        });
    }

    public CompanyFake WithIndustryId(Guid industryId)
    {
        RuleFor(x => x.IndustryId, industryId);
        return this;
    }

    public CompanyFake WithName(string name)
    {
        RuleFor(x => x.Name, name);
        return this;
    }

    public CompanyFake AsDeleted()
    {
        RuleFor(x => x.IsDeleted, true);
        return this;
    }
}
