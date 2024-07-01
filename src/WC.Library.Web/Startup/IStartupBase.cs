using Autofac;
using Microsoft.AspNetCore.Builder;

namespace WC.Library.Web.Startup;

public interface IStartupBase
{
    void ConfigureServices(WebApplicationBuilder builder);
    
    void ConfigureContainer(ContainerBuilder builder);
    
    void Configure(WebApplication app);
}