using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using WC.Library.Employee.Shared.Exceptions;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     The handler for <see cref="AuthenticationFailedException"/>.
/// </summary>
public sealed class AuthenticationExceptionHandler : ExceptionHandlerBase<AuthenticationException>
{
    public override ErrorDto GetError(
        Exception exception)
    {
        return new ErrorDto
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Authentication Failed",
            Description = exception.Message
        };
    }
}
