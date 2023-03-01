using Autofac;

namespace S1XViewer.Storage
{
    public class HandlerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {   
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsImplementedInterfaces()
                .Where(tp => tp.Name.ToUpper().EndsWith("STORAGE"))
                .InstancePerLifetimeScope();
        }
    }
}
