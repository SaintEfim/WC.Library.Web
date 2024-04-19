using Microsoft.Extensions.Configuration;
using WC.Library.Shared.Constants;
using WC.Library.Web.Models;

namespace WC.Library.Web.Infrastructure.ExceptionHandling.Helpers;

public class BuildErrorDtoHelper
{
    private readonly bool _stackTraceReflections;

    public BuildErrorDtoHelper(IConfiguration config)
    {
        _stackTraceReflections = config.GetValue<bool>("ErrorSettings:StackTrace:Disabled");
    }

    public ErrorDto BuildErrorDto(string title, int statusCode, Exception exception)
    {
        var errors = new Dictionary<string, string[]> { { exception.GetType().Name, [exception.Message] } };
        var errorDto = new ErrorDto
        {
            Type = ErrorSettingsConstants.ErrorUrlType,
            Title = title,
            Status = statusCode,
            Errors = errors,
            StackTrace = _stackTraceReflections ? null : exception.StackTrace
        };
        return errorDto;
    }
}