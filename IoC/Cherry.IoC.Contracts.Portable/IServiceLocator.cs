using System;

namespace Cherry.IoC.Contracts.Portable
{
    public interface IServiceLocator
    {
        object Get(Type serviceKey, params InjectionParameter[] parameters);
        bool CanGet(Type serviceKey);
    }
}
