using Microsoft.AspNetCore.Http;
using WC.Library.Shared.Exceptions;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     The handler for <see cref="NotFoundException"/>.
/// </summary>
public sealed class NotFoundExceptionHandler : ExceptionHandlerBase<NotFoundException>
{
    public override ErrorDto GetError(
        Exception exception)
    {
        return new ErrorDto
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Entity not found",
            Description = exception.Message
        };
    }
}
