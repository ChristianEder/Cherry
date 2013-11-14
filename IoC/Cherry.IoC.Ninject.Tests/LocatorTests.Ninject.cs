using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Ninject;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        private IServiceRegistry CreateRegistry()
        {
            return new NinjectServiceRegistry();
        }

        private static void AssertTransitiveDependencyHasCorrectLocatorInjected(BarUsingSomething bar, IServiceLocator registeredIn, IServiceLocator resolvedFrom)
        {
            // Ninject resolves transitive dependencies from 
            // the kernel the dependency was registered in
            Assert.AreSame(registeredIn, bar.Something.Locator);
        }
    }
}
