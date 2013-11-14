using System;
using System.Linq;
using System.Reflection;
using Cherry.IoC.Common.Portable;
using Cherry.IoC.Contracts.Portable;
using Microsoft.Practices.Unity;
using InjectionParameter = Cherry.IoC.Contracts.Portable.InjectionParameter;

namespace Cherry.IoC.Unity
{
    internal class UnityServiceLocator : IServiceLocator
    {
        private readonly IUnityContainer _container;
        private readonly UnityServiceLocator _parent;

        public UnityServiceLocator(IUnityContainer container, UnityServiceLocator parent)
        {
            _container = container;
            _parent = parent;
        }

        public object Get(Type serviceKey, params InjectionParameter[] parameters)
        {
            object factoryMethod;
            if (ServiceLocatorFactoryMethodSupportExtensions.GetFactoryMethod(this, serviceKey, out factoryMethod))
            {
                return factoryMethod;
            }

            if (parameters == null || parameters.Length == 0)
            {
                return _container.Resolve(serviceKey);
            }

            ContainerRegistration registration = GetRegistration(serviceKey);

            if (registration != null && registration.LifetimeManager != null &&
                !(registration.LifetimeManager is PerResolveLifetimeManager))
            {
                throw new ArgumentException(
                    string.Format("Cannot use injection parameters when using a LifetimeManager for service \"{0}\" other than PerResolveLifetimeManager", serviceKey),
                    "parameters");
            }

            ResolverOverride[] ps = ModifyParameters(serviceKey, registration, parameters);
            return _container.Resolve(serviceKey, ps);
        }

        public bool CanGet(Type serviceKey)
        {
            if (_container.IsRegistered(serviceKey))
            {
                return true;
            }
            IUnityContainer parent = _container.Parent;
            while (parent != null)
            {
                if (parent.IsRegistered(serviceKey))
                {
                    return true;
                }
                parent = parent.Parent;
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



        private ResolverOverride[] ModifyParameters(Type serviceKey, ContainerRegistration registration,
            InjectionParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return null;
            }
            Type resolvedType = null;
            return
                parameters.Select(
                    p =>
                        string.IsNullOrEmpty(p.Key)
                            ? ResolveParameterName(serviceKey, registration, ref resolvedType, p)
                            : new ParameterOverride(p.Key, p.Value))
                    .ToArray();
        }

        private ResolverOverride ResolveParameterName(Type serviceKey, ContainerRegistration registration,
            ref Type resolvedType, InjectionParameter injectionParameter)
        {
            resolvedType = resolvedType ?? TypeToGetResolved(serviceKey, registration);
            ParameterInfo constructorParameter = resolvedType.GetConstructors().SelectMany(c => c.GetParameters())
                .First(p => p.ParameterType.IsInstanceOfType(injectionParameter.Value));
            return new ParameterOverride(constructorParameter.Name, injectionParameter.Value);
        }

        private Type TypeToGetResolved(Type serviceKey, ContainerRegistration registration)
        {
            if (registration != null)
            {
                return registration.MappedToType;
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract)
            {
                return serviceKey;
            }
            return null;
        }

        private ContainerRegistration GetRegistration(Type serviceKey)
        {
            ContainerRegistration registered =
                _container.Registrations.FirstOrDefault(r => r.RegisteredType == serviceKey);
            if (registered != null)
            {
                return registered;
            }

            ContainerRegistration parent = _parent != null ? _parent.GetRegistration(serviceKey) : null;
            if (parent != null)
            {
                return parent;
            }

            return null;
        }
    }
}