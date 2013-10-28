using System;

namespace Cherry.IoC.Contracts.Portable
{
    public interface IServiceRegistry
    {
        void Register(Type serviceKey, object service);
        void Register(Type serviceKey, Type serviceType, bool singleton);
        IServiceRegistry CreateChildRegistry();
        IServiceLocator Locator { get; }
        bool IsRegistered(Type serviceKey);
    }
}