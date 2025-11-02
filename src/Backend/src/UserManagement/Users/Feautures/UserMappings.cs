using UserManagement.Users.Feautures.CompletingUserRegistration.V1;
using UserManagement.Users.Models;

namespace UserManagement.Users.Feautures;

public static class UserMappings
{
    public static User ToModel(
        this CompleteUserRegistrationCommand command,
        Guid id,
        string passwordHash,
        Guid companyId
    )
    {
        ArgumentNullException.ThrowIfNull(command);

        return new User
        {
            Id = id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.UserName,
            Email = command.Email,
            CompanyId = companyId,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
        };
    }


    public static CompleteUserRegistrationCommand ToCommand(this CompleteUserRegistrationRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CompleteUserRegistrationCommand(
            request.CompanyId,
            request.FirstName,
            request.LastName,
            request.UserName,
            request.Password,
            request.PasswordRepetition,
            request.Email,
            request.AcceptTermsOfService,
            request.AcceptPrivacyPolicy
        );
    }
}