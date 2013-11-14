using System;
using Cherry.IoC.Cherry.Portable.Resolver;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable
{
    internal interface ICherryServiceLocatorAndRegistry : IServiceRegistry, IServiceLocator
    {
        object Get(ICherryServiceLocatorAndRegistry originalLocator, Type serviceKey, params InjectionParameter[] parameters);
    }
}