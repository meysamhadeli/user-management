using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Industries.Exceptions;

public class IndustryAlreadyExistsException : AppException
{
    public IndustryAlreadyExistsException(string industryName)
        : base($"Industry '{industryName}' already exists!", HttpStatusCode.Conflict)
    {
    }
}