using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Ninject;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace Cherry.IoC.Tests
{
    public partial class LocatorTests
    {
        [TestMethod]
        public void ResolveUnityContainerFromLocator()
        {
            var childRegistry = _registry.CreateChildRegistry();

            var rootKernel = _registry.Locator.Get<IKernel>();
            var childKernelTyped = childRegistry.Locator.Get<IChildKernel>();
            var childKernel = childRegistry.Locator.Get<IKernel>();

            Assert.IsNotNull(rootKernel);
            Assert.IsNotNull(childKernel);
            Assert.IsNotNull(childKernelTyped);
            Assert.AreSame(childKernel, childKernelTyped);
            Assert.IsNotNull(childKernelTyped.ParentResolutionRoot);
            Assert.AreNotSame(rootKernel, childKernel);
            Assert.AreSame(rootKernel, childKernelTyped.ParentResolutionRoot);
        }

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
