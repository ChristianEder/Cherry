using System;

namespace Cherry.IoC.Contracts.Portable
{
    public interface IServiceRegistry : IDisposable
    {
        void Register(Type serviceKey, object service);
        void Register(Type serviceKey, Type serviceType, bool singleton);
        IServiceRegistry CreateChildRegistry();
        IServiceRegistry Parent { get; }
        IServiceLocator Locator { get; }
        bool IsRegistered(Type serviceKey);
    }
}