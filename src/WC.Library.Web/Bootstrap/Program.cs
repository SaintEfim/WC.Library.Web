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

        appBuilder.Logging.ClearProviders();
        appBuilder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });

        var startup = Activator.CreateInstance(typeof(TStartup), appBuilder) as TStartup;
        if (startup == null) throw new ArgumentNullException(nameof(startup));

        appBuilder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);

        startup.ConfigureServices(appBuilder);

        var app = appBuilder.Build();

        startup.Configure(app);

        await app.RunAsync();
    }
}