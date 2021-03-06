﻿using System;
using System.Diagnostics;
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
            _registry = CreateRegistry();
            Assert.IsNotNull(_registry, "Please implement the method CreateRegistry()");
            _locator = _registry.Locator;
        }

        #region General

        [TestMethod]
        public void ResolveRegistry()
        {
            var registry = _locator.Get<IServiceRegistry>();
            Assert.AreSame(_registry, registry);
        }

        [TestMethod]
        public void ResolveLocator()
        {
            var locator = _locator.Get<IServiceLocator>();
            Assert.AreSame(_locator, locator);
        }

        [TestMethod]
        public void ResolveClass()
        {
            var foo = _locator.Get<Foo>();
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void CanResolveClass()
        {
            var canFoo = _locator.CanGet<Foo>();
            Assert.IsTrue(canFoo);
        }

        [TestMethod]
        public void CanResolveClassFactoryMethod()
        {
            var canFoo = _locator.CanGet<Func<Foo>>();
            Assert.IsTrue(canFoo);
        }

        [TestMethod]
        public void ResolveFactoryMethod()
        {
            _registry.Register<IFoo, Foo>(true);

            var foo1 = _locator.Get<IFoo>();
            var foo2Func = _locator.Get<Func<IFoo>>();
            var foo2 = foo2Func();

            Assert.IsNotNull(foo1);
            Assert.IsNotNull(foo2);
            Assert.AreSame(foo1, foo2);
        }
        
        [TestMethod]
        public void CanGet()
        {
            Assert.IsFalse(_locator.CanGet(typeof(IFoo)));
            Assert.IsTrue(_locator.CanGet(typeof(Foo)));
            _registry.Register<IFoo, Foo>(false);
            Assert.IsTrue(_locator.CanGet(typeof(IFoo)));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanGetWithNullKeyFails()
        {
            _locator.CanGet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ResolveUnregisteredInterfaceFails()
        {
            _locator.Get<IFoo>();
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

        #region Parametrized Gets

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ParametrizedGetFailsOnSingletons()
        {
            _registry.Register<IBar, BarWithStringParameter>(true);
            _locator.With("Hello World").Get<IBar>();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ParametrizedGetFailsOnInstances()
        {
            _registry.Register<IBar>(new BarWithStringParameter("Something"));
            _locator.With("Hello World").Get<IBar>();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ParametrizedGetFailsWhenParameterIsMissing()
        {
            _registry.Register<IBar, BarWithStringParameter>(false);
            var bar = _locator.Get<IBar>();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ParametrizedGetFailsWhenParameterIsNotNamedCorrectly()
        {
            _registry.Register<IBar, BarWithStringParameter>(false);
            var bar = _locator.With("notTheNameOfTheParameter", "Hello World").Get<IBar>();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ParametrizedGetMultipleParametersMissingNamesFailing()
        {
            _registry.Register<IBar, BarWithMultipleParameters>(false);
            _registry.Register<IFoo, Foo>(true);

            var bar = _locator
                .With("Hello")
                .With(17)
                .Get<IBar>();
        }

        [TestMethod]
        public void ParametrizedGetSingleStringParameter()
        {
            _registry.Register<IBar, BarWithStringParameter>(false);
            var bar = _locator.With("Hello World").Get<IBar>();

            Assert.IsNotNull(bar);
            Assert.IsInstanceOfType(bar, typeof(BarWithStringParameter));
            Assert.AreEqual("Hello World", ((BarWithStringParameter)bar).Parameter);
        }

        [TestMethod]
        public void ParametrizedGetMultipleParameters()
        {
            _registry.Register<IBar, BarWithMultipleParameters>(false);
            _registry.Register<IFoo, Foo>(false);
            var foo = new Foo();
            var bar = _locator
                .With("parameter1", foo)
                .With("parameter2", "Hello")
                .With("parameter3", "World")
                .With("parameter4", 17)
                .Get<IBar>();

            Assert.IsNotNull(bar);
            var typedBar = bar as BarWithMultipleParameters;
            Assert.IsNotNull(typedBar);

            Assert.AreSame(foo, typedBar.Parameter1);
            Assert.AreEqual("Hello", typedBar.Parameter2);
            Assert.AreEqual("World", typedBar.Parameter3);
            Assert.AreEqual(17, typedBar.Parameter4);
        }

        [TestMethod]
        public void ParametrizedGetMultipleParametersSomeOfThemInjected()
        {
            _registry.Register<IBar, BarWithMultipleParameters>(false);
            _registry.Register<IFoo, Foo>(true);
           
            var bar = _locator
                .With("parameter2", "Hello")
                .With("parameter3", "World")
                .With(17)
                .Get<IBar>();

            Assert.IsNotNull(bar);
            var typedBar = bar as BarWithMultipleParameters;
            Assert.IsNotNull(typedBar);

            var foo = _locator.Get<IFoo>();
            Assert.AreSame(foo, typedBar.Parameter1);
            Assert.AreEqual("Hello", typedBar.Parameter2);
            Assert.AreEqual("World", typedBar.Parameter3);
            Assert.AreEqual(17, typedBar.Parameter4);
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

            Assert.IsInstanceOfType(bar1, typeof(BarUsingSomething));
            Assert.IsInstanceOfType(bar2, typeof(BarUsingSomething));
            Assert.AreNotSame(bar1, bar2);

            var bar1Typed = (BarUsingSomething)bar1;
            var bar2Typed = (BarUsingSomething)bar2;

            Assert.AreSame(childLocator, bar1Typed.Locator);
            Assert.AreSame(childLocator, bar2Typed.Locator);


            if (transitiveIsSingleton)
            {
                Assert.AreSame(bar1Typed.Something, bar2Typed.Something);
            }
            else
            {
                Assert.AreNotSame(bar1Typed.Something, bar2Typed.Something);
            }

            // Different IoC Frameworks like Ninject and Unity handle
            // the resolving of transitive dependencies differently.
            // Thus, we make that assertion in the framework-specific code
            AssertTransitiveDependencyHasCorrectLocatorInjected(bar1Typed, _locator, childLocator);
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
    }
}