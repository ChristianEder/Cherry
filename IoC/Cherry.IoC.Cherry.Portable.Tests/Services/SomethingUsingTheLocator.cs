using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Tests.Services
{
    public class SomethingUsingTheLocatorAndRegistry : ISomethingUsingTheLocatorAndRegistry
    {
        public IServiceLocator Locator { get; private set; }
        public IServiceRegistry Registry { get; private set; }

        public SomethingUsingTheLocatorAndRegistry(IServiceLocator locator, IServiceRegistry registry)
        {
            Locator = locator;
            Registry = registry;
        }
    }
}
