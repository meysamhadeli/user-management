using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Users.Exceptions;

public class UsernameAlreadyExistsException : AppException
{
    public UsernameAlreadyExistsException(string username) 
        : base($"Username '{username}' already exists!", HttpStatusCode.Conflict)
    {
    }
}