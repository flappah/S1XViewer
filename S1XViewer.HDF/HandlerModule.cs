using Autofac;
using S1XViewer.HDF.Interfaces;

namespace S1XViewer.HDF
{
    public class HandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                   .AsImplementedInterfaces()
                   .Where(tp => tp.Name.ToUpper().EndsWith("PRODUCTSUPPORT") ||
                                tp.Name.ToUpper().EndsWith("READER"))
                   .AsImplementedInterfaces()           
                   .InstancePerLifetimeScope();

            List<Type> supports =
               ThisAssembly.GetTypes().ToList()
                   .Where(tp => !tp.IsInterface &&
                                !tp.IsAbstract &&
                                tp.Name.ToUpper().EndsWith("PRODUCTSUPPORT"))
                   .Distinct()
                   .ToList();

            builder.Register(c => new ProductSupportFactory
            {
                Supports = (from support in supports
                            select support.GetInterface("I" + support.Name)
                            into typeInterface
                            select c.Resolve(typeInterface) as IProductSupportBase).ToArray()
            }).As<IProductSupportFactory>().InstancePerLifetimeScope();

        }
    }
}
