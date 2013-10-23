using System;
using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    [TestClass]
    public partial class LocatorTests
    {
        private IServiceRegistry _registry;
        private IServiceLocator _locator;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _registry = null;
            CreateRegistry();
            Assert.IsNotNull(_registry, "Please implement the partial method CreateRegistry()");
            _locator = _registry.Locator;
        }

        #region General

        [TestMethod]
        public void ResolveClass()
        {
            var foo = _locator.Get<Foo>();
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void InjectLocatorAndRegistry()
        {
            _registry.Register<ISomethingUsingTheLocatorAndRegistry, SomethingUsingTheLocatorAndRegistry>(false);

            var something = _locator.Get<ISomethingUsingTheLocatorAndRegistry>();
            Assert.IsNotNull(something);
            Assert.IsNotNull(something.Locator);
            Assert.IsNotNull(something.Registry);
            Assert.AreSame(_locator, something.Locator);
            Assert.AreSame(_registry, something.Registry);
        }

        #endregion

        #region Tests with child registries

        [TestMethod]
        public void TransitiveSingletonDependencyResolvedFromRootContainer()
        {
            TestTransitiveDependencyResolvedFromRootContainer(true);
        }

        [TestMethod]
        public void TransitivePerResolveDependencyResolvedFromRootContainer()
        {
            TestTransitiveDependencyResolvedFromRootContainer(false);
        }

        private void TestTransitiveDependencyResolvedFromRootContainer(bool transitiveIsSingleton)
        {
            var childRegistry = _registry.CreateChildRegistry();
            var childLocator = childRegistry.Locator;

            _registry.Register<ISomethingUsingTheLocatorAndRegistry, SomethingUsingTheLocatorAndRegistry>(transitiveIsSingleton);

            childRegistry.Register<IBar, BarUsingSomething>(false);

            var bar1 = childLocator.Get<IBar>();
            var bar2 = childLocator.Get<IBar>();

            Assert.IsInstanceOfType(bar1, typeof (BarUsingSomething));
            Assert.IsInstanceOfType(bar2, typeof (BarUsingSomething));
            Assert.AreNotSame(bar1, bar2);

            var bar1Typed = (BarUsingSomething) bar1;
            var bar2Typed = (BarUsingSomething) bar2;

            Assert.AreSame(childLocator, bar1Typed.Locator);
            Assert.AreSame(childLocator, bar2Typed.Locator);

            if (transitiveIsSingleton)
            {
                Assert.AreSame(bar1Typed.Something, bar2Typed.Something);
                Assert.AreSame(_locator, bar1Typed.Something.Locator);
            }
            else
            {
                Assert.AreNotSame(bar1Typed.Something, bar2Typed.Something);
                Assert.AreSame(childLocator, bar1Typed.Something.Locator);
                Assert.AreSame(childLocator, bar2Typed.Something.Locator);
            }
        }

        #endregion

        #region GetInstance

        [TestMethod]
        public void GetInstance()
        {
            var foo = new Foo();
            _registry.Register<IFoo>(foo);

            TestInstance<IFoo>(foo, _locator);
        }

        [TestMethod]
        public void GetInstanceFromChildLocator()
        {
            var childRegistry = _registry.CreateChildRegistry();
            var childLocator = childRegistry.Locator;

            var foo = new Foo();
            _registry.Register<IFoo>(foo);

            TestInstance<IFoo>(foo, childLocator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInstanceNullKeyFails()
        {
            var foo = new Foo();
            _registry.Register<IFoo>(foo);

            _locator.Get(null);
        }

        private void TestInstance<T>(T foo, IServiceLocator serviceLocator)
        {
            var resolved = serviceLocator.Get(typeof(T));
            var resolvedTyped = serviceLocator.Get<T>();

            Assert.AreSame(foo, resolved);
            Assert.AreSame(foo, resolvedTyped);
        }

        #endregion

        #region GetSingleton

        [TestMethod]
        public void GetSingleton()
        {
            _registry.Register<IFoo, Foo>(true);

            TestSingleton<IFoo>(_locator);
        }

        [TestMethod]
        public void GetSingletonFromChildLocator()
        {
            var childRegistry = _registry.CreateChildRegistry();
            var childLocator = childRegistry.Locator;

            _registry.Register<IFoo, Foo>(true);

            TestSingleton<IFoo>(childLocator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSingletonNullKeyFails()
        {
            _registry.Register<IFoo, Foo>(true);

            _locator.Get(null);
        }

        private void TestSingleton<T>(IServiceLocator serviceLocator)
        {
            var resolved = serviceLocator.Get(typeof(T));
            var resolvedTyped = serviceLocator.Get<T>();

            Assert.IsNotNull(resolved);
            Assert.IsNotNull(resolvedTyped);
            Assert.AreSame(resolved, resolvedTyped);
        }

        #endregion

        #region GetPerResolve

        [TestMethod]
        public void GetPerResolve()
        {
            _registry.Register<IFoo, Foo>(false);
            TestPerResolve<IFoo>(_locator);
        }

        [TestMethod]
        public void GetPerResolveFromChildLocator()
        {
            var childRegistry = _registry.CreateChildRegistry();
            var childLocator = childRegistry.Locator;

            _registry.Register<IFoo, Foo>(false);

            TestPerResolve<IFoo>(childLocator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPerResolveNullKeyFails()
        {
            _registry.Register<IFoo, Foo>(true);

            _locator.Get(null);
        }

        private void TestPerResolve<T>(IServiceLocator locator)
        {
            var resolved = locator.Get(typeof(T));
            var resolvedTyped = locator.Get<T>();

            Assert.IsNotNull(resolved);
            Assert.IsNotNull(resolvedTyped);
            Assert.IsInstanceOfType(resolved, typeof(T));
            Assert.AreNotSame(resolved, resolvedTyped);
        }

        #endregion

        partial void CreateRegistry();
    }
}