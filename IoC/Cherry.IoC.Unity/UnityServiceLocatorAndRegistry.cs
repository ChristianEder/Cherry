using System;
using System.Linq;
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
            return new UnityServiceLocatorAndRegistry(_container.CreateChildContainer());
        }

        public IServiceLocator Locator
        {
            get { return this; }
        }

        public bool IsRegistered(Type serviceKey)
        {
            return IsRegisteredIn(serviceKey, _container);
        }

        public object Get(Type serviceKey)
        {
            return _container.Resolve(serviceKey);
        }

        public bool CanGet(Type serviceKey)
        {
            if (IsRegisteredIn(serviceKey, _container))
            {
                return true;
            }
            var parent = _container.Parent;
            while (parent != null)
            {
                if (IsRegisteredIn(serviceKey, parent))
                {
                    return true;
                }
                parent = parent.Parent;
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract)
            {
                return serviceKey.GetConstructors().Any(c => c.GetParameters().All(p => CanGet(p.ParameterType)));
            }
            return false;
        }

        private static bool IsRegisteredIn(Type serviceKey, IUnityContainer container)
        {
            return container.IsRegistered(serviceKey);
        }
    }
}