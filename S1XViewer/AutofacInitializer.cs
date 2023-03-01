using Autofac;

namespace S1XViewer
{
    public static class AutofacInitializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new S1XViewer.Base.HandlerModule());
            builder.RegisterModule(new S1XViewer.Types.HandlerModule());
            builder.RegisterModule(new S1XViewer.Storage.HandlerModule());
            builder.RegisterModule(new S1XViewer.HDF.HandlerModule());
            builder.RegisterModule(new S1XViewer.Model.HandlerModule());

            var container = builder.Build();

            return container;
        }
    }
}
