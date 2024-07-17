using Microsoft.AspNetCore.Http;
using WC.Library.Employee.Shared.Exceptions;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     The handler for <see cref="PasswordMismatchException"/>.
/// </summary>
public sealed class PasswordMismatchExceptionHandler : ExceptionHandlerBase<PasswordMismatchException>
{
    public override ErrorDto GetError(
        Exception exception)
    {
        return new ErrorDto
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Password Mismatch",
            Description = exception.Message
        };
    }
}