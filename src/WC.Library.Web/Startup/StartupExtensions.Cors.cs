using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace WC.Library.Web.Startup;

public static partial class StartupExtensions
{
    public static void UseCors(
        this IApplicationBuilder app,
        IConfiguration config)
    {
        var urlCors = config.GetValue<string>("Cors:AllowedOrigins");

        if (string.IsNullOrEmpty(urlCors))
        {
            throw new Exception("Cors not configured");
        }

        app.UseCors(builder => builder.WithOrigins(urlCors)
            .AllowAnyMethod()
            .AllowAnyHeader());
    }
}
