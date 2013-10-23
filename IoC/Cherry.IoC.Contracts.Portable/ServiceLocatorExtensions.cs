namespace Cherry.IoC.Contracts.Portable
{
    public static class ServiceLocatorExtensions
    {
        public static TService Get<TService>(this IServiceLocator serviceLocator)
        {
            var service = serviceLocator.Get(typeof(TService));
            return (TService)service;
        }
    }
}