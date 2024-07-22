using Autofac;
using Microsoft.Extensions.Configuration;
using WC.Library.Web.Configuration;
using WC.Library.Web.Middleware;

namespace WC.Library.Web;

public sealed class WcLibraryWebModule : Module
{
    protected override void Load(
        ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(ThisAssembly)
            .AssignableTo<IExceptionHandler>()
            .AsImplementedInterfaces()
            .SingleInstance()
            .Keyed<IExceptionHandler>(a => a.BaseType!.GenericTypeArguments[0]);

        builder.Register(c =>
            {
                var config = c.Resolve<IConfiguration>();
                return new AuthenticationConfiguration(config);
            })
            .SingleInstance();
    }
}
