using Autofac;
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
    }
}