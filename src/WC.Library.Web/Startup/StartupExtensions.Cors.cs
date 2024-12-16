using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace WC.Library.Web.Startup;

public static partial class StartupExtensions
{
    public static void UseCors(
        this IApplicationBuilder app,
        IConfiguration config)
    {
        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        if (allowedOrigins.Length == 0)
        {
            throw new Exception("Cors не настроен.");
        }

        app.UseCors(builder =>
            builder.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader());
    }
}
