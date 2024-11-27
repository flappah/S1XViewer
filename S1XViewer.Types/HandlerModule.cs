using Autofac;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class HandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsImplementedInterfaces()
                .Where(tp => tp.GetInterfaces().ToList().Select(e => e?.FullName?.EndsWith(".IFeature")) != null)
                .InstancePerLifetimeScope();

            List<Type> abstractConcreteTypes =
                ThisAssembly.GetTypes().ToList()
                    .Where(tp => tp.IsInterface == false && tp.IsAbstract == false)
                    .Distinct()
                    .ToList();

            List<Type> featureTypes = new List<Type>();
            foreach (Type type in abstractConcreteTypes)
            {
                var interfaces = type.GetInterfaces().ToList();
                foreach (Type? intf in interfaces)
                {
                    if (intf.FullName?.EndsWith(".IFeature") == true)
                    {
                        featureTypes.Add(type);
                    }
                }
            };

            builder.RegisterType<FeatureRendererManager>().As(typeof(IFeatureRendererManager)).InstancePerLifetimeScope();

            builder.Register(c => new FeatureFactory()
            {
                Features = (from featureType in featureTypes
                            select featureType.GetInterface("I" + featureType.Name)
                            into typeInterface
                            select c.Resolve(typeInterface) as IFeature).ToArray()
            }).As<IFeatureFactory>().InstancePerLifetimeScope();
        }
    }
}
