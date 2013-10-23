using System;
using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.IoC.Tests
{
    [TestClass]
    public partial class RegistryTests
    {
        private IServiceRegistry _registry;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _registry = null;
            CreateRegistry();
            Assert.IsNotNull(_registry, "Please implement the partial method CreateRegistry()");
        }

        [TestMethod]
        public void HasLocator()
        {
            Assert.IsNotNull(_registry.Locator);
        }

        [TestMethod]
        public void CreateChildRegistry()
        {
            var child = _registry.CreateChildRegistry();
            Assert.IsNotNull(child);
        }

        #region RegisterInstance

        [TestMethod]
        public void RegisterInstance()
        {
            _registry.Register(typeof(IFoo), new Foo());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterInstanceNullInstanceFails()
        {
            _registry.Register(typeof(IFoo), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterInstanceNullKeyFails()
        {
            _registry.Register(null, new Foo());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterInstanceWronglyTypedInstanceFails()
        {
            _registry.Register(typeof(IBar), new Foo());
        }

        #endregion

        #region RegisterSingleton

        [TestMethod]
        public void RegisterSingleton()
        {
            _registry.Register(typeof(IFoo), typeof(Foo), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingletonNullTypeFails()
        {
            _registry.Register(typeof(IFoo), null, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingletonNullKeyFails()
        {
            _registry.Register(null, typeof(Foo), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingletonWronglyTypedTypeFails()
        {
            _registry.Register(typeof(IBar), typeof(Foo), true);
        }

        #endregion

        #region RegisterPerResolve

        [TestMethod]
        public void RegisterPerResolve()
        {
            _registry.Register(typeof(IFoo), typeof(Foo), false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterPerResolveNullTypeFails()
        {
            _registry.Register(typeof(IFoo), null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterPerResolveNullKeyFails()
        {
            _registry.Register(null, typeof(Foo), false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPerResolveWronglyTypedTypeFails()
        {
            _registry.Register(typeof(IBar), typeof(Foo), false);
        }

        #endregion

        partial void CreateRegistry();
    }
}
