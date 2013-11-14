using System;
using System.Collections.Generic;
using System.Linq;
using Cherry.IoC.Cherry.Portable.Resolver;
using Cherry.IoC.Common.Portable;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable
{
    public class CherryServiceLocatorAndRegistry : ICherryServiceLocatorAndRegistry
    {
        private readonly CherryServiceLocatorAndRegistry _parent;
        private readonly List<CherryServiceLocatorAndRegistry> _children = new List<CherryServiceLocatorAndRegistry>(); 

        private readonly Dictionary<Type, IResolver> _registrations = new Dictionary<Type, IResolver>();

        public CherryServiceLocatorAndRegistry()
            : this(null)
        {
        }

        private CherryServiceLocatorAndRegistry(CherryServiceLocatorAndRegistry parent)
        {
            _parent = parent;
            Register(typeof(IServiceLocator), this);
            Register(typeof(IServiceRegistry), this);
            Register(typeof(ICherryServiceLocatorAndRegistry), this);
        }

        public void Register(Type serviceKey, object service)
        {
            if (ReferenceEquals(service, null))
            {
                throw new ArgumentNullException("service", "The service instance must not be null");
            }
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }
            if (!serviceKey.IsInstanceOfType(service))
            {
                throw new ArgumentException("The service instance must be convertible to the type specified as serviceKey", "service");
            }
            var singletonInstanceResolver = new SingletonInstanceResolver(service);
            Register(serviceKey, singletonInstanceResolver);
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
            if (!serviceKey.IsAssignableFrom(serviceType))
            {
                throw new ArgumentException("The serviceType must be convertible to the type specified as serviceKey", "serviceType");
            }

            IResolver resolver;
            if (singleton)
            {
                resolver = new SingletonResolver(serviceType);
            }
            else
            {
                resolver = new PerResolveResolver(serviceType);
            }
            Register(serviceKey, resolver);
        }

        public IServiceRegistry CreateChildRegistry()
        {
            var child = new CherryServiceLocatorAndRegistry(this);

            lock (_children)
            {
                _children.Add(child);
            }

            return child;
        }

        public IServiceRegistry Parent
        {
            get
            {
                return _parent;
            }
        }

        public IServiceLocator Locator
        {
            get { return this; }
        }

        public bool IsRegistered(Type serviceKey)
        {
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }
            return _registrations.ContainsKey(serviceKey);
        }

        public object Get(Type serviceKey, params InjectionParameter[] parameters)
        {
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }
            return GetInternal(this, serviceKey, parameters);
        }

        public bool CanGet(Type serviceKey)
        {
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }
            if (IsRegisteredRecursively(serviceKey))
            {
                return true;
            }

            Type factoryMethodType;
            if (ServiceLocatorFactoryMethodSupportExtensions.IsFactoryMethod(serviceKey, out factoryMethodType))
            {
                return CanGet(factoryMethodType);
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract)
            {
                return serviceKey.GetConstructors().Any(c => c.GetParameters().All(p => CanGet(p.ParameterType)));
            }
            return false;
        }

        object ICherryServiceLocatorAndRegistry.Get(ICherryServiceLocatorAndRegistry originalLocator, Type serviceKey, params InjectionParameter[] parameters)
        {
            return GetInternal(originalLocator, serviceKey, parameters);
        }

        private object GetInternal(ICherryServiceLocatorAndRegistry originalLocator, Type serviceKey,
            params InjectionParameter[] parameters)
        {
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
            }

            object factoryMethod;
            if (ServiceLocatorFactoryMethodSupportExtensions.GetFactoryMethod(this, serviceKey, out factoryMethod))
            {
                return factoryMethod;
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract && !IsRegisteredRecursively(serviceKey))
            {
                var perResolveResolver = new PerResolveResolver(serviceKey);
                var instance = perResolveResolver.Get(originalLocator, this, parameters);
                return instance;
            }

            IResolver resolver;
            if (!_registrations.TryGetValue(serviceKey, out resolver))
            {
                if (_parent != null)
                {
                    return _parent.GetInternal(originalLocator, serviceKey);
                }
                throw new ArgumentException(string.Format("The type {0} could not be resolved.", serviceKey), "serviceKey");
            }
            return resolver.Get(originalLocator, this, parameters);
        }

        public void Dispose()
        {
            lock (_registrations)
            {
                var resolvers = _registrations.Values.ToArray();
                _registrations.Clear();
                foreach (var resolver in resolvers)
                {
                    resolver.Dispose();
                }
            }
            lock (_children)
            {
                foreach (var child in _children)
                {
                    child.Dispose();
                }
                _children.Clear();
            }
        }

        private bool IsRegisteredRecursively(Type serviceKey)
        {
            if (IsRegistered(serviceKey))
            {
                return true;
            }
            return _parent != null && _parent.IsRegisteredRecursively(serviceKey);
        }
    }
}
