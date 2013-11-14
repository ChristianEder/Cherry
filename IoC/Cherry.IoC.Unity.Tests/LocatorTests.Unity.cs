using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Cherry.IoC.Unity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        [TestMethod]
        public void ResolveUnityContainerFromLocator()
        {
            var childRegistry = _registry.CreateChildRegistry();

            var rootUnity = _registry.Locator.Get<IUnityContainer>();
            var childUnity = childRegistry.Locator.Get<IUnityContainer>();

            Assert.IsNotNull(rootUnity);
            Assert.IsNotNull(childUnity);
            Assert.IsNotNull(childUnity.Parent);
            Assert.AreNotSame(rootUnity, childUnity);
            Assert.AreSame(rootUnity, childUnity.Parent);
        }

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
