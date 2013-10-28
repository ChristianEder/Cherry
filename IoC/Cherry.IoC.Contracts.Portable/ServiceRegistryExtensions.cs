namespace Cherry.IoC.Contracts.Portable
{
    public static class ServiceRegistryExtensions
    {
        public static IServiceRegistry Register<TServiceKey>(this IServiceRegistry registry, TServiceKey service)
        {
            registry.Register(typeof (TServiceKey), service);
            return registry;
        }

        public static bool IsRegistered<TServiceKey>(this IServiceRegistry registry)
        {
            return registry.IsRegistered(typeof(TServiceKey));
        }

        public static IServiceRegistry Register<TServiceKey, TService>(this IServiceRegistry registry, bool singleton)
        {
            registry.Register(typeof(TServiceKey), typeof(TService), singleton);
            return registry;
        }

        public static IServiceRegistry Load<TModule>(this IServiceRegistry registry)
            where TModule : IModule, new()
        {
            registry.Load(new TModule());
            return registry;
        }

        public static IServiceRegistry Load<TModule>(this IServiceRegistry registry, TModule module)
            where TModule : IModule
        {
            module.Load(registry);
            return registry;
        }
    }
}