using System.Net;
using BuildingBlocks.Exception;

namespace UserManagement.Users.Exceptions;

public class TermsNotAcceptedException : AppException
{
    public TermsNotAcceptedException() 
        : base("You must accept both terms of service and privacy policy", HttpStatusCode.BadRequest)
    {
    }
}