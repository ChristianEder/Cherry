using System;
using Cherry.IoC.Contracts.Portable;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;

namespace Cherry.IoC.Ninject
{
    public class NinjectServiceRegistry : IServiceRegistry
    {
        private readonly IKernel _kernel;
        private readonly NinjectServiceRegistry _parent;
        private bool _hasBeenDisposed;
        private readonly NinjectServiceLocator _locator;

          public NinjectServiceRegistry() : this(null)
        {
        }

          private NinjectServiceRegistry(NinjectServiceRegistry parent)
        {
            _parent = parent;
              if (_parent != null)
              {
                  var childKernel = new ChildKernel(_parent._kernel);
                  _kernel = childKernel;
                  _kernel.Bind<IChildKernel>().ToConstant(childKernel);
              }
              else
              {
                  _kernel = new StandardKernel();
              }
              _locator = new NinjectServiceLocator(_kernel, _parent != null ? _parent._locator : null);
            _kernel.Bind<IServiceRegistry>().ToConstant(this);
            _kernel.Bind<IServiceLocator>().ToConstant(_locator);
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
                  throw new ArgumentException(
                      "The service instance must be convertible to the type specified as serviceKey", "service");
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
                  throw new ArgumentException("The serviceType must be convertible to the type specified as serviceKey",
                      "serviceType");
              }

              IBindingWhenInNamedWithOrOnSyntax<object> binding = _kernel.Bind(serviceKey).To(serviceType);
              if (singleton)
              {
                  binding.InSingletonScope();
              }
          }

          public IServiceRegistry CreateChildRegistry()
          {
              return new NinjectServiceRegistry(this);
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
              return _kernel.IsBound(serviceKey);
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