using Cherry.IoC.Cherry.Portable;
using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        private IServiceRegistry CreateRegistry()
        {
            return new CherryServiceLocatorAndRegistry();
        }

        private static void AssertTransitiveDependencyHasCorrectLocatorInjected(BarUsingSomething bar, IServiceLocator registeredIn, IServiceLocator resolvedFrom)
        {
            // Unity resolves transitive dependencies from 
            // the container the original resolve was called on
            Assert.AreSame(registeredIn, bar.Something.Locator);
        }
    }
}
