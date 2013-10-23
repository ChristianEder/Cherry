using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Tests.Services
{
    public class Bar : IBar
    {
        public void Foo()
        {
        }
    }

    public class BarUsingSomething : IBar
    {
        public ISomethingUsingTheLocatorAndRegistry Something { get; private set; }
        public IServiceLocator Locator { get; private set; }

        public BarUsingSomething(ISomethingUsingTheLocatorAndRegistry something, IServiceLocator locator)
        {
            Something = something;
            Locator = locator;
        }

        public void Foo()
        {
        }
    }
}