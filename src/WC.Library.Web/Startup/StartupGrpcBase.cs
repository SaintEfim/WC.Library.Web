using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WC.Library.Web.Helpers;

namespace WC.Library.Web.Startup;

public abstract class StartupGrpcBase : IStartupBase
{
    private readonly Lazy<Assembly[]> _assemblies = new(AssemblyHelpers.GetApplicationAssemblies);

    protected StartupGrpcBase(
        WebApplicationBuilder builder)
    {
        Configuration = builder.Configuration;
    }

    protected IConfiguration Configuration { get; }

    public virtual void ConfigureServices(
        WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddGrpc();
        services.AddAutoMapper(_assemblies.Value);
    }

    public virtual void ConfigureContainer(
        ContainerBuilder containerBuilder)
    {
    }

    public virtual void Configure(
        WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.MapGet("/", () => { });
    }
}
