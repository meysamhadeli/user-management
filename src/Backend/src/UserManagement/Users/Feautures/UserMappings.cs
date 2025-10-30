using UserManagement.Users.Dtos;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;
using UserManagement.Users.Models;

namespace UserManagement.Users.Feautures;

public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.UserName,
            user.Email,
            user.CompanyId,
            user.Company.Name);
    }

    public static User ToModel(this CompleteUserRegistrationCommand command, Guid id, string passwordHash, Guid companyId)
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
                   CreatedAt = DateTime.UtcNow
               };
    }
}