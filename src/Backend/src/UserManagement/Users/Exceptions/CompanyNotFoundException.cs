using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Users.Exceptions;

public class CompanyNotFoundException : AppException
{
    public CompanyNotFoundException(Guid companyId) 
        : base($"Company with ID '{companyId}' does not exist", HttpStatusCode.NotFound)
    {
    }
}