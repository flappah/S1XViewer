using Autofac;
using S1XViewer.Model.Geometry;
using S1XViewer.Model.Interfaces;

namespace S1XViewer.Model
{
    public class HandlerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .As(type => type.GetInterfaces())
                .Where(tp =>
                    tp.Name.ToUpper().EndsWith("LOADER") ||
                    tp.Name.ToUpper().EndsWith("DATAPARSER") ||
                    tp.Name.ToUpper().EndsWith("BUILDER"))
                .InstancePerLifetimeScope();

            List<Type> geometryBuilders =
                ThisAssembly.GetTypes().ToList()
                    .Where(tp => !tp.IsInterface &&
                                 !tp.IsAbstract &&
                                 tp.Name.ToUpper().EndsWith("BUILDER"))
                    .Distinct()
                    .ToList();

            List<Type> dataParsers =
                ThisAssembly.GetTypes().ToList()
                    .Where(tp => !tp.IsInterface &&
                                 !tp.IsAbstract &&
                                 tp.Name.ToUpper().EndsWith("DATAPARSER"))
                    .Distinct()
                    .ToList();

            builder.RegisterType<FeatureRendererFactory>().As(typeof(IFeatureRendererFactory)).InstancePerLifetimeScope();

            builder.Register(c => new GeometryBuilderFactory
            {
                Builders = (from geometryBuilder in geometryBuilders
                            select geometryBuilder.GetInterface("I" + geometryBuilder.Name)
                            into typeInterface
                            select c.Resolve(typeInterface) as IGeometryBuilder).ToArray()
            }).As<IGeometryBuilderFactory>().InstancePerLifetimeScope();

            builder.Register(c => new DataPackageParser
            {
                DataParsers = (from dataParser in dataParsers
                               select dataParser.GetInterface("I" + dataParser.Name)
                               into typeInterface
                               select c.Resolve(typeInterface) as IDataParser).ToArray()
            }).As<IDataPackageParser>().InstancePerLifetimeScope();
        }
    }
}
