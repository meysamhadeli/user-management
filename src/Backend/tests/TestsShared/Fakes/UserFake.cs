using Bogus;
using BuildingBlocks.Utils;
using UserManagement.Users.Models;

namespace IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;

/// <summary>
/// Creates a fake `User` with valid data adhering to domain rules.
/// </summary>
public sealed class UserFake : Faker<User>
{
    public UserFake(Guid companyId, string? username = null, string? email = null)
    {
        CustomInstantiator(f =>
        {
            return new User
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                FirstName = f.Name.FirstName(),
                LastName = f.Name.LastName(),
                UserName = username ?? f.Internet.UserName(),
                Email = email ?? f.Internet.Email(),
                PasswordHash = PasswordHasher.HashPassword("TestPassword123!"),
                CreatedAt = f.Date.Past(),
                CreatedBy = f.Random.Long(1, 1000),
                LastModified = f.Date.Recent(),
                LastModifiedBy = f.Random.Long(1, 1000),
                IsDeleted = false,
                Version = 1,
            };
        });
    }

    public UserFake WithCompanyId(Guid companyId)
    {
        RuleFor(x => x.CompanyId, companyId);
        return this;
    }

    public UserFake WithUserName(string username)
    {
        RuleFor(x => x.UserName, username);
        return this;
    }

    public UserFake WithEmail(string email)
    {
        RuleFor(x => x.Email, email);
        return this;
    }

    public UserFake WithPassword(string password)
    {
        RuleFor(x => x.PasswordHash, PasswordHasher.HashPassword(password));
        return this;
    }

    public UserFake AsDeleted()
    {
        RuleFor(x => x.IsDeleted, true);
        return this;
    }
}