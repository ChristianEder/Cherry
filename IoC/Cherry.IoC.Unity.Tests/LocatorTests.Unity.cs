using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Cherry.IoC.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        private IServiceRegistry CreateRegistry()
        {
            return new UnityServiceRegistry();
        }

        private static void AssertTransitiveDependencyHasCorrectLocatorInjected(BarUsingSomething bar, IServiceLocator registeredIn, IServiceLocator resolvedFrom)
        {
            // Unity resolves transitive dependencies from 
            // the container the original resolve was called on
            Assert.AreSame(resolvedFrom, bar.Something.Locator);
        }
    }
}
