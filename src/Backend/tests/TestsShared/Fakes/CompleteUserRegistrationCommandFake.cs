using Bogus;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;

namespace IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;

/// <summary>
/// Creates a fake `CompleteUserRegistrationCommand` with valid data.
/// </summary>
public sealed class CompleteUserRegistrationCommandFake : Faker<CompleteUserRegistrationCommand>
{
    public CompleteUserRegistrationCommandFake(Guid companyId, string? username = null, string? email = null)
    {
        CustomInstantiator(f =>
        {
            return new CompleteUserRegistrationCommand(
                CompanyId: companyId,
                FirstName: f.Name.FirstName(),
                LastName: f.Name.LastName(),
                UserName: username ?? f.Internet.UserName(),
                Password: "ValidPassword123!",
                PasswordRepetition: "ValidPassword123!",
                Email: email ?? f.Internet.Email(),
                AcceptTermsOfService: true,
                AcceptPrivacyPolicy: true
            );
        });
    }

    public CompleteUserRegistrationCommandFake WithCompanyId(Guid companyId)
    {
        RuleFor(x => x.CompanyId, companyId);
        return this;
    }

    public CompleteUserRegistrationCommandFake WithUserName(string username)
    {
        RuleFor(x => x.UserName, username);
        RuleFor(x => x.Password, "ValidPassword123!");
        RuleFor(x => x.PasswordRepetition, "ValidPassword123!");
        return this;
    }

    public CompleteUserRegistrationCommandFake WithEmail(string email)
    {
        RuleFor(x => x.Email, email);
        return this;
    }

    public CompleteUserRegistrationCommandFake WithPassword(string password)
    {
        RuleFor(x => x.Password, password);
        RuleFor(x => x.PasswordRepetition, password);
        return this;
    }

    public CompleteUserRegistrationCommandFake WithMismatchedPasswords()
    {
        RuleFor(x => x.Password, "Password123!");
        RuleFor(x => x.PasswordRepetition, "DifferentPassword123!");
        return this;
    }

    public CompleteUserRegistrationCommandFake WithoutTermsAcceptance()
    {
        RuleFor(x => x.AcceptTermsOfService, false);
        return this;
    }

    public CompleteUserRegistrationCommandFake WithoutPrivacyPolicyAcceptance()
    {
        RuleFor(x => x.AcceptPrivacyPolicy, false);
        return this;
    }
}
