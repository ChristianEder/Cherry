using System;

namespace Cherry.IoC.Contracts.Portable
{
    public static class ServiceLocatorExtensions
    {
        public static TService Get<TService>(this IServiceLocator serviceLocator, params InjectionParameter[] parameters)
        {
            var service = serviceLocator.Get(typeof(TService), parameters);
            return (TService)service;
        }

        public static bool CanGet<TServiceKey>(this IServiceLocator serviceLocator)
        {
            return serviceLocator.CanGet(typeof(TServiceKey));
        }

        public static TService TryGet<TService>(this IServiceLocator serviceLocator, params InjectionParameter[] parameters)
        {
            if (serviceLocator.CanGet<TService>())
            {
                try
                {
                    return serviceLocator.Get<TService>(parameters);
                }
                catch (Exception)
                {
                    return default(TService);
                }
            }
            return default(TService);
        }

        public static IServiceLocator With<TValue>(this IServiceLocator serviceLocator, TValue value)
        {
            var parametrizedServiceLocator = GetOrCreateParametrizedServiceLocator(serviceLocator);
            return parametrizedServiceLocator.With(value);
        }

        public static IServiceLocator With<TValue>(this IServiceLocator serviceLocator, string key, TValue value)
        {
            var parametrizedServiceLocator = GetOrCreateParametrizedServiceLocator(serviceLocator);
            return parametrizedServiceLocator.With(key, value);
        }

        private static IParametrizedServiceLocator GetOrCreateParametrizedServiceLocator(IServiceLocator serviceLocator)
        {
            if (ReferenceEquals(serviceLocator, null))
            {
                throw new ArgumentNullException("serviceLocator", "The serviceLocator must not be null");
            }

            var parametrizedServiceLocator = (serviceLocator as IParametrizedServiceLocator) ??
                                             new ParametrizedServiceLocator(serviceLocator);
            return parametrizedServiceLocator;
        }
    }
}