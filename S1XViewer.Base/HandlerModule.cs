using Autofac;
using System.Linq;

namespace S1XViewer.Base
{
    public class HandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .As(type => type.GetInterfaces())
                .Where(tp =>
                    tp.Name.ToUpper().Contains("INJECTABLE"))
                .InstancePerLifetimeScope();
        }
    }
}
