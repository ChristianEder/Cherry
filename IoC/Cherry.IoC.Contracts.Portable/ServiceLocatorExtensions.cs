using System;

namespace Cherry.IoC.Contracts.Portable
{
    public static class ServiceLocatorExtensions
    {
        public static TService Get<TService>(this IServiceLocator serviceLocator)
        {
            var service = serviceLocator.Get(typeof(TService));
            return (TService)service;
        }

        public static bool CanGet<TServiceKey>(this IServiceLocator serviceLocator)
        {
            return serviceLocator.CanGet(typeof(TServiceKey));
        }

        public static TService TryGet<TService>(this IServiceLocator serviceLocator)
        {
            if (serviceLocator.CanGet<TService>())
            {
                try
                {
                    return serviceLocator.Get<TService>();
                }
                catch (Exception)
                {
                    return default(TService);
                }
            }
            return default(TService);
        }

    }
}