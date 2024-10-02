using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace WC.Library.Web.Startup;

public static partial class StartupExtensions
{
    public static void AddSwagger(
        this IServiceCollection services)
    {
        services.AddOpenApiDocument(configure =>
        {
            configure.Title = Assembly.GetEntryAssembly()
                ?.GetName()
                .Name;
            configure.Version = "v1";

            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                configure.PostProcess = document =>
                {
                    document.Info.Title = Assembly.GetEntryAssembly()
                        ?.GetName()
                        .Name;
                    document.Info.Version = "v1";
                    document.Info.Description = "API Documentation";
                };
            }

            configure.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}.",
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
    }
}
