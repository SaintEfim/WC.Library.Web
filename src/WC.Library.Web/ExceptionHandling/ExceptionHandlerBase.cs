using WC.Library.Web.Middleware;
using WC.Library.Web.Models;

namespace WC.Library.Web.ExceptionHandling;

/// <summary>
///     Base exception handler implementation.
/// </summary>
/// <typeparam name="T">The type of the exception to be handled.</typeparam>
public abstract class ExceptionHandlerBase<T> : IExceptionHandler
    where T : Exception
{
    /// <inheritdoc/>
    public Type ExceptionType { get; } = typeof(T);

    /// <inheritdoc/>
    public abstract ErrorDto GetError(
        Exception exception);
}