using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Companies.Exceptions;

public class IndustryNotFoundException : AppException
{
    public IndustryNotFoundException(Guid industryId)
        : base($"Industry with ID '{industryId}' does not exist", HttpStatusCode.NotFound)
    {
    }
}