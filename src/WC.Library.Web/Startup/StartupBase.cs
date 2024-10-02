using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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

    public virtual void ConfigureServices(
        WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddCors();
        services.AddGrpc();
        services.AddControllers()
            .AddNewtonsoftJson();

        services.AddAutoMapper(_assemblies.Value);

        services.AddSwagger();
        services.AddAuthentication(Configuration);

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
        app.UseCors(Configuration);
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
}
