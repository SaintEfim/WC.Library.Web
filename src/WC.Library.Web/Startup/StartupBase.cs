using System.Reflection;
using System.Text;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WC.Library.Shared.Constants;
using WC.Library.Web.Helpers;
using WC.Library.Web.Infrastructure.ExceptionHandling;
using WC.Library.Web.Infrastructure.ExceptionHandling.Extensions;
using WC.Library.Web.Infrastructure.ExceptionHandling.Helpers;

namespace WC.Library.Web.Startup;

public abstract class StartupBase
{
    protected IConfiguration Configuration { get; }

    protected StartupBase(WebApplicationBuilder builder)
    {
        Configuration = builder.Configuration;
    }

    private readonly Lazy<Assembly[]> _assemblies = new(AssemblyHelpers.GetApplicationAssemblies);

    internal virtual void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddExceptionMappingFromAllAssemblies();
        services.AddControllers();
        services.AddAutoMapper(_assemblies.Value);

        ConfigureSwagger(services);
        ConfigureAuthentication(services);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public virtual void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<BuildErrorDtoHelper>().SingleInstance();
        builder.RegisterType<GetTitleAndStatusCodeHelper>().SingleInstance();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo { Title = Assembly.GetEntryAssembly()?.GetName().Name, Version = "v1" });

            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition(BearerTokenConstants.TokenType, new OpenApiSecurityScheme
            {
                Description = BearerTokenConstants.DescriptionToken,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = BearerTokenConstants.TokenType
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = BearerTokenConstants.TokenType
                        },
                        Scheme = "oauth2",
                        Name = BearerTokenConstants.TokenType,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetValue<string>("ApiSettings:AccessSecret")!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
}