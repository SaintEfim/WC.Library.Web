using System.Reflection;
using System.Text;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using WC.Library.Web.Configuration;
using WC.Library.Web.Helpers;
using WC.Library.Web.Middleware;

namespace WC.Library.Web.Startup;

public abstract class StartupBase
{
    private readonly Lazy<Assembly[]> _assemblies = new(AssemblyHelpers.GetApplicationAssemblies);

    protected StartupBase(
        WebApplicationBuilder builder)
    {
        Configuration = builder.Configuration;
    }

    protected IConfiguration Configuration { get; }

    private AuthenticationConfiguration AuthenticationConfiguration => new(Configuration);

    public virtual void ConfigureServices(
        WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddGrpc();
        services.AddControllers()
            .AddNewtonsoftJson();

        services.AddAutoMapper(_assemblies.Value);

        ConfigureSwagger(services);
        ConfigureAuthentication(services);

        services.AddProblemDetails();

        services.AddAuthorization();
    }

    public virtual void ConfigureContainer(
        ContainerBuilder builder)
    {
        builder.RegisterModule<WcLibraryWebModule>();
    }

    public virtual void Configure(
        WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapControllers();
    }

    private static void ConfigureSwagger(
        IServiceCollection services)
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

    private void ConfigureAuthentication(
        IServiceCollection services)
    {
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticationConfiguration.AccessSecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }
}
