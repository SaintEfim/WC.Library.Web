using Microsoft.AspNetCore.Http;
using WC.Library.Employee.Shared.Exceptions;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     The handler for <see cref="RegistrationFailedException"/>.
/// </summary>
public sealed class RegistrationFailedExceptionHandler : ExceptionHandlerBase<RegistrationFailedException>
{
    public override ErrorDto GetError(
        Exception exception)
    {
        return new ErrorDto
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Registration Failed",
            Description = exception.Message
        };
    }
}
