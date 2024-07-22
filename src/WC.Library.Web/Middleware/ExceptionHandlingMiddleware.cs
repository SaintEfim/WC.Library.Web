using System.Net.Mime;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using WC.Library.Web.Models;

namespace WC.Library.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly IHostEnvironment _environment;

    private readonly IIndex<Type, IExceptionHandler> _exceptionHandlers;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IIndex<Type, IExceptionHandler> exceptionHandlers,
        IHostEnvironment environment)
    {
        _next = next;
        _exceptionHandlers = exceptionHandlers;
        _environment = environment;
    }

    public async Task InvokeAsync(
        HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleException(httpContext, ex);
        }
    }

    private async Task HandleException(
        HttpContext httpContext,
        Exception exception)
    {
        ErrorDto error;
        try
        {
            error = GetError(exception);

            if (_environment.IsDevelopment())
            {
                error.StackTrace = GetStackTrace(httpContext, exception);
            }
        }
        catch (Exception e)
        {
            error = GetDefaultError(e);
        }

        await WriteErrorDtoToResponseAsync(httpContext, error);
    }

    private ErrorDto GetError(
        Exception exception)
    {
        var exceptionHandler = GetExceptionHandler(exception);
        if (exceptionHandler == default)
        {
            return GetDefaultError(exception);
        }

        var error = exceptionHandler.GetError(exception);
        var errorDto = new ErrorDto
        {
            Title = error.Title,
            Description = error.Description,
            Status = error.Status,
            Extensions = error.Extensions
        };

        return errorDto;
    }

    private IExceptionHandler? GetExceptionHandler(
        Exception exception)
    {
        var exceptionType = exception.GetType();
        while (exceptionType != null)
        {
            if (_exceptionHandlers.TryGetValue(exceptionType, out var exceptionHandler))
            {
                return exceptionHandler;
            }

            exceptionType = exceptionType.BaseType;
        }

        return null;
    }

    private static async Task WriteErrorDtoToResponseAsync(
        HttpContext httpContext,
        ErrorDto errorDto)
    {
        httpContext.Response.StatusCode = errorDto.Status;
        var response = JsonConvert.SerializeObject(errorDto,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new StringEnumConverter() }
            });
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        await httpContext.Response.WriteAsync(response);
    }

    private static ErrorDto GetDefaultError(
        Exception exception)
    {
        return new ErrorDto
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Extensions = { { "Message", exception.Message } }
        };
    }

    private static IEnumerable<string> GetStackTrace(
        HttpContext httpContext,
        Exception exception)
    {
        var stackTrace = new List<string>();

        if (httpContext.Request != default!)
        {
            stackTrace.Add($"RequestUrl: {httpContext.Request.Method} {httpContext.Request.GetEncodedUrl()}");
        }

        stackTrace.Add($"{exception.GetType().FullName} was thrown: {exception.Message}");

        stackTrace.AddRange(exception.StackTrace
            ?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .ToList() ?? Enumerable.Empty<string>());

        return stackTrace;
    }
}
