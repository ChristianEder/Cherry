using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Tests.Services
{
    public interface ISomethingUsingTheLocatorAndRegistry
    {
        IServiceLocator Locator { get; }
        IServiceRegistry Registry { get; }
    }
}