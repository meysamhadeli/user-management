using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Users.Exceptions;

public class EmailAlreadyExistsException : AppException
{
    public EmailAlreadyExistsException(string email)
        : base($"Email '{email}' already exists!", HttpStatusCode.Conflict) { }
}