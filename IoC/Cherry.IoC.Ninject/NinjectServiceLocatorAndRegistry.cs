using System;
using Cherry.IoC.Contracts.Portable;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace Cherry.IoC.Ninject
{
    public class NinjectServiceLocatorAndRegistry : IServiceRegistry, IServiceLocator
    {
        private readonly IKernel _kernel;

        public NinjectServiceLocatorAndRegistry() : this(new StandardKernel())
        {

        }

        private NinjectServiceLocatorAndRegistry(IKernel kernel)
        {
            _kernel = kernel;
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
                throw new ArgumentException("The service instance must be convertible to the type specified as serviceKey" ,"service");
            }
            _kernel.Bind(serviceKey).ToConstant(service);
        }

        public void Register(Type serviceKey, Type serviceType, bool singleton)
        {
            if (ReferenceEquals(serviceType, null))
            {
                throw new ArgumentNullException("serviceType", "The serviceType must not be null");
            }
            if (ReferenceEquals(serviceKey, null))
            {
                throw new ArgumentNullException("serviceKey", "The serviceKey must not be null");
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
            IKernel child = new ChildKernel(_kernel);
            return new NinjectServiceLocatorAndRegistry(child);
        }

        public IServiceLocator Locator
        {
            get { return this; }
        }

        public object Get(Type serviceKey)
        {
            return _kernel.Get(serviceKey);
        }
    }
}
