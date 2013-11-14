using System;
using System.Linq;
using Cherry.IoC.Common.Portable;
using Cherry.IoC.Contracts.Portable;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Extensions.ChildKernel;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Cherry.IoC.Ninject
{
    internal class NinjectServiceLocator : IServiceLocator
    {
        private readonly IKernel _kernel;
        private readonly NinjectServiceLocator _parent;

        public NinjectServiceLocator(IKernel kernel, NinjectServiceLocator parent)
        {
            _kernel = kernel;
            _parent = parent;
        }

        public object Get(Type serviceKey, params InjectionParameter[] parameters)
        {
            object factoryMethod;
            if (ServiceLocatorFactoryMethodSupportExtensions.GetFactoryMethod(this, serviceKey,
                out factoryMethod))
            {
                return factoryMethod;
            }

            if (parameters == null || parameters.Length == 0)
            {
                return _kernel.Get(serviceKey);
            }

            var binding = GetBinding(serviceKey);

            if (binding != null &&
                (binding.Target != BindingTarget.Type ||
                 binding.ScopeCallback != StandardScopeCallbacks.Transient))
            {
                throw new ArgumentException("Cannot use injection parameters when using a lifetimemanager for service " + serviceKey, "parameters");
            }
            var ps = ModifyParameters(serviceKey, binding, parameters);
            return _kernel.Get(serviceKey, ps);
        }

        public bool CanGet(Type serviceKey)
        {
            if (_kernel.IsBound(serviceKey))
            {
                return true;
            }
            var kernelAsChildKernel = _kernel as ChildKernel;
            IKernel parent = kernelAsChildKernel != null ? kernelAsChildKernel.ParentResolutionRoot as IKernel : null;

            while (parent != null)
            {
                if (_kernel.IsBound(serviceKey))
                {
                    return true;
                }
                kernelAsChildKernel = parent as ChildKernel;
                parent = kernelAsChildKernel != null ? kernelAsChildKernel.ParentResolutionRoot as IKernel : null;
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




        private IParameter[] ModifyParameters(Type serviceKey, IBinding binding, InjectionParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return null;
            }
            Type resolvedType = null;
            return
                parameters.Select(
                    p => string.IsNullOrEmpty(p.Key) ? ResolveParameterName(serviceKey, binding, ref resolvedType, p) : new ConstructorArgument(p.Key, p.Value))
                    .ToArray();
        }

        private IParameter ResolveParameterName(Type serviceKey, IBinding binding, ref Type resolvedType, InjectionParameter injectionParameter)
        {
            resolvedType = resolvedType ?? TypeToGetResolved(serviceKey, binding);
            var constructorParameter = resolvedType.GetConstructors().SelectMany(c => c.GetParameters())
                .First(p => p.ParameterType.IsInstanceOfType(injectionParameter.Value));
            return new ConstructorArgument(constructorParameter.Name, injectionParameter.Value);
        }

        private Type TypeToGetResolved(Type serviceKey, IBinding binding)
        {
            if (binding != null)
            {
                if (binding.Target != BindingTarget.Type && binding.Target != BindingTarget.Self)
                {
                    throw new InvalidOperationException("Cannot use unnamed parameters for getting types that are not bound to self or to another type");
                }
                try
                {

                    var req = _kernel.CreateRequest(serviceKey, metadata => true, new IParameter[0], true, false);
                    var cache = _kernel.Components.Get<ICache>();
                    var planner = _kernel.Components.Get<IPlanner>();
                    var pipeline = _kernel.Components.Get<IPipeline>();
                    var provider = binding.GetProvider(new Context(_kernel, req, binding, cache, planner, pipeline));
                    return provider.Type;
                }
                catch (Exception)
                {
                    return binding.GetType();
                }
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract)
            {
                return serviceKey;
            }
            return null;
        }

        private IBinding GetBinding(Type serviceKey)
        {
            var registered = _kernel.GetBindings(serviceKey).FirstOrDefault();
            if (registered != null)
            {
                return registered;
            }

            var parent = _parent != null ? _parent.GetBinding(serviceKey) : null;
            if (parent != null)
            {
                return parent;
            }

            return null;
        }
    }
}