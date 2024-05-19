using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using WC.Library.Web.Startup;

namespace WC.Library.Web.Bootstrap;

public class Program<TStartup> where TStartup : StartupBase
{
    public static async Task Main(string[] args)
    {
        var appBuilder = WebApplication.CreateBuilder(args);
        var app = WebApplication.Create();
        appBuilder.Logging.ClearProviders();

        var startup = Activator.CreateInstance(typeof(TStartup), appBuilder) as TStartup;

        appBuilder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(startup!.ConfigureContainer)
            .UseSerilog((
                context,
                loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

        startup.ConfigureServices(appBuilder);

        await startup.Configure(app);
    }
}