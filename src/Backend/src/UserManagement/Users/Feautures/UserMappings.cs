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
}
