using FluentValidation;
using Microsoft.AspNetCore.Http;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     The handler for <see cref="FluentValidation.ValidationException"/>.
/// </summary>
public sealed class FluentValidationExceptionHandler : ExceptionHandlerBase<ValidationException>
{
    public override ErrorDto GetError(
        Exception exception)
    {
        return new ErrorDto
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "The operation can't be processed",
            Description = exception.Message
        };
    }
}
