using System;
using Cherry.IoC.Contracts.Portable;
using Microsoft.Practices.Unity;

namespace Cherry.IoC.Unity
{
    public class UnityServiceLocatorAndRegistry : IServiceRegistry, IServiceLocator
    {
        private readonly IUnityContainer _container;

        public UnityServiceLocatorAndRegistry() : this(new UnityContainer())
        {
        }

        private UnityServiceLocatorAndRegistry(IUnityContainer container)
        {
            _container = container;
            _container.RegisterInstance<IServiceRegistry>(this);
            _container.RegisterInstance<IServiceLocator>(this);
        }

        public void Register(Type serviceKey, object service)
        {
            _container.RegisterInstance(serviceKey, service);
        }

        public void Register(Type serviceKey, Type serviceType, bool singleton)
        {
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }

            LifetimeManager lifetimeManager;
            if (singleton)
            {
                lifetimeManager = new ContainerControlledLifetimeManager();
            }
            else
            {
                lifetimeManager = new PerResolveLifetimeManager();
            }

            _container.RegisterType(serviceKey, serviceType, lifetimeManager);
        }

        public IServiceRegistry CreateChildRegistry()
        {
            return new UnityServiceLocatorAndRegistry(_container.CreateChildContainer());
        }

        public IServiceLocator Locator
        {
            get { return this; }
        }

        public object Get(Type serviceKey)
        {
            return _container.Resolve(serviceKey);
        }
    }
}