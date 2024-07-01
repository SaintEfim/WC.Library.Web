using WC.Library.Web.Models;

namespace WC.Library.Web.Middleware;

public interface IExceptionHandler
{
    ErrorDto GetError(Exception exception);
}