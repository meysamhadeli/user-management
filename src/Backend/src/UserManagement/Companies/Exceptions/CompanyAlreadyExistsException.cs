using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Companies.Exceptions;

public class CompanyAlreadyExistsException : AppException
{
    public CompanyAlreadyExistsException(string companyName) 
        : base($"Company '{companyName}' already exists!", HttpStatusCode.Conflict)
    {
    }
}