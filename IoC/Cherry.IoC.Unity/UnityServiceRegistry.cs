using System;
using Cherry.IoC.Contracts.Portable;
using Microsoft.Practices.Unity;

namespace Cherry.IoC.Unity
{
    public class UnityServiceRegistry : IServiceRegistry
    {
        private readonly IUnityContainer _container;
        private readonly UnityServiceRegistry _parent;
        private bool _hasBeenDisposed;
        private readonly UnityServiceLocator _locator;

        public UnityServiceRegistry() : this(null)
        {
        }

        private UnityServiceRegistry(UnityServiceRegistry parent)
        {
            _parent = parent;
            _container = _parent != null ? _parent._container.CreateChildContainer() : new UnityContainer();
            _container.RegisterInstance<IServiceRegistry>(this);
            _locator = new UnityServiceLocator(_container, _parent != null ? _parent._locator : null);
            _container.RegisterInstance<IServiceLocator>(Locator);
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
            if (ReferenceEquals(serviceType, null))
            {
                throw new ArgumentNullException("serviceType", "The serviceType must not be null");
            }
            if (!serviceType.IsClass || serviceType.IsAbstract)
            {
                throw new ArgumentException("The serviceType must be a non-abstract class type", "serviceType");
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
            return new UnityServiceRegistry(this);
        }

        public IServiceRegistry Parent
        {
            get { return _parent; }
        }

        public IServiceLocator Locator
        {
            get { return _locator; }
        }

        public bool IsRegistered(Type serviceKey)
        {
            return _container.IsRegistered(serviceKey);
        }

        public void Dispose()
        {
            if (_hasBeenDisposed)
            {
                return;
            }
            _hasBeenDisposed = true;
            _container.Dispose();
        }
    }
}