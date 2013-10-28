using System;
using System.Linq;
using Cherry.IoC.Contracts.Portable;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace Cherry.IoC.Ninject
{
    public class NinjectServiceLocatorAndRegistry : IServiceRegistry, IServiceLocator
    {
        private readonly IKernel _kernel;
        private readonly NinjectServiceLocatorAndRegistry _parent;
        private bool _hasBeenDisposed;

        public NinjectServiceLocatorAndRegistry() : this(null)
        {

        }

        private NinjectServiceLocatorAndRegistry(NinjectServiceLocatorAndRegistry parent)
        {
            _parent = parent;
            _kernel = _parent != null ? new ChildKernel(_parent._kernel) : new StandardKernel();
            _kernel.Bind<IServiceRegistry>().ToConstant(this);
            _kernel.Bind<IServiceLocator>().ToConstant(this);
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
            _kernel.Bind(serviceKey).ToConstant(service);
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

            var binding = _kernel.Bind(serviceKey).To(serviceType);
            if (singleton)
            {
                binding.InSingletonScope();
            }
        }

        public IServiceRegistry CreateChildRegistry()
        {
            return new NinjectServiceLocatorAndRegistry(this);
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
            return IsRegisteredIn(serviceKey, _kernel);
        }
        public object Get(Type serviceKey)
        {
            return _kernel.Get(serviceKey);
        }

        public bool CanGet(Type serviceKey)
        {
            if (IsRegisteredIn(serviceKey, _kernel))
            {
                return true;
            }
            var kernelAsChildKernel = _kernel as ChildKernel;
            var parent = kernelAsChildKernel != null ? kernelAsChildKernel.ParentResolutionRoot as IKernel : null;

            while (parent != null)
            {
                if (IsRegisteredIn(serviceKey, parent))
                {
                    return true;
                }
                kernelAsChildKernel = parent as ChildKernel;
                parent = kernelAsChildKernel != null ? kernelAsChildKernel.ParentResolutionRoot as IKernel : null;
            }

            if (serviceKey.IsClass && !serviceKey.IsAbstract)
            {
                return serviceKey.GetConstructors().Any(c => c.GetParameters().All(p => CanGet(p.ParameterType)));
            }
            return false;
        }

        private static bool IsRegisteredIn(Type serviceKey, IKernel kernel)
        {
            return kernel.GetBindings(serviceKey).Any();
        }

        public void Dispose()
        {
            if (_hasBeenDisposed)
            {
                return;
            }
            _hasBeenDisposed = true;
            _kernel.Dispose();
        }
    }
}
