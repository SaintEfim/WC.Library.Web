using System.Net;
using WC.Library.Shared.Constants;

namespace WC.Library.Web.Infrastructure.ExceptionHandling.Helpers;

public class GetTitleAndStatusCodeHelper
{
    private readonly Dictionary<Type, (string title, int statusCode)> _exceptionMapping;

    public GetTitleAndStatusCodeHelper(Dictionary<Type, (string title, int statusCode)> exceptionMapping)
    {
        _exceptionMapping = exceptionMapping;
    }

    public (string, int) GetTitleAndStatusCode(Exception exception)
    {
        string title;
        int statusCode;

        if (_exceptionMapping.TryGetValue(exception.GetType(), out var mapping))
        {
            title = mapping.title;
            statusCode = mapping.statusCode;
        }
        else
        {
            title = ErrorSettingsConstants.DefaultTitle;
            statusCode = (int)HttpStatusCode.InternalServerError;
        }

        return (title, statusCode);
    }
}