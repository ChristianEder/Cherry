namespace Cherry.IoC.Contracts.Portable
{
    public static class ServiceRegistryExtensions
    {
        public static IServiceRegistry Register<TServiceKey>(this IServiceRegistry registry, TServiceKey service)
        {
            registry.Register(typeof (TServiceKey), service);
            return registry;
        }

        public static IServiceRegistry Register<TServiceKey, TService>(this IServiceRegistry registry, bool singleton)
        {
            registry.Register(typeof (TServiceKey), typeof (TService), singleton);
            return registry;
        }
    }
}