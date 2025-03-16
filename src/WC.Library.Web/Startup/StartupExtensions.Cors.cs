using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace WC.Library.Web.Startup;

public static partial class StartupExtensions
{
    public static void UseCors(
        this IApplicationBuilder app,
        IConfiguration config)
    {
        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (allowedOrigins == null || allowedOrigins.Length == 0)
        {
            throw new Exception("CORS is not configured. Make sure the allowed origins are defined in the configuration.");
        }

        Console.WriteLine($"CORS is configured for the following origins: {string.Join(", ", allowedOrigins)}");

        app.UseCors(builder =>
            builder.WithOrigins(allowedOrigins)
                   .AllowAnyMethod()
                   .AllowAnyHeader());
    }
}

