using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WC.Library.Web.Helpers;

namespace WC.Library.Web.ExceptionHandling;

internal static class ExceptionMappingExtensions
{
    public static void AddExceptionMappingFromAllAssemblies(this IServiceCollection services)
    {
        var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Union(new[] { Assembly.GetExecutingAssembly() });

        var exceptionTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                typeof(Exception).IsAssignableFrom(type) &&
                type is { IsAbstract: false, IsInterface: false, IsClass: true }
            );

        services.AddSingleton(MapExceptionsHelper.MapExceptions(exceptionTypes));
    }
}