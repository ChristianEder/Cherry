using System;
using Cherry.IoC.Contracts.Portable;
using Cherry.IoC.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            _registry = CreateRegistry();
            Assert.IsNotNull(_registry, "Please implement the method CreateRegistry()");
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

        [TestMethod]
        public void ParentRegistryDispose()
        {
            var child = _registry.CreateChildRegistry();

            child.Register<IFoo, Foo>(true);
            var foo = (Foo)child.Locator.Get<IFoo>();

            _registry.Dispose();
            Assert.AreEqual(1, foo.DisposedCalls);
        }

        #region RegisterInstance

        [TestMethod]
        public void RegisterInstance()
        {
            _registry.Register(typeof(IFoo), new Foo());
        }

        [TestMethod]
        public void IsRegistered()
        {
            Assert.IsFalse(_registry.IsRegistered(typeof(IFoo)));
            _registry.Register(typeof(IFoo), new Foo());
            Assert.IsTrue(_registry.IsRegistered(typeof(IFoo)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsRegisteredWithNullKeyFails()
        {
            _registry.IsRegistered(null);
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

        [TestMethod]
        public void RegisterInstanceDispose()
        {
            var foo = new Foo();
            _registry.Register(foo);

            _registry.Dispose();

            Assert.AreEqual(1, foo.DisposedCalls);
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingletonInterfaceFails()
        {
            _registry.Register(typeof(IFoo), typeof(IFoo), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingletonAbstractClassFails()
        {
            _registry.Register(typeof(IFoo), typeof(AbstractFoo), true);
        }

        [TestMethod]
        public void RegisterSingletonDispose()
        {
            _registry.Register<IFoo, Foo>(true);
            var foo = (Foo)_registry.Locator.Get<IFoo>();
            _registry.Dispose();
            Assert.AreEqual(1, foo.DisposedCalls);
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPerResolveInterfaceFails()
        {
            _registry.Register(typeof(IFoo), typeof(IFoo), false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPerResolveAbstractClassFails()
        {
            _registry.Register(typeof(IFoo), typeof(AbstractFoo), false);
        }

        [TestMethod]
        public void RegisterPerResolveDispose()
        {
            _registry.Register<IFoo, Foo>(false);
            var foo = (Foo)_registry.Locator.Get<IFoo>();
            _registry.Dispose();
            Assert.AreEqual(0, foo.DisposedCalls);
        }

        #endregion
    }
}
