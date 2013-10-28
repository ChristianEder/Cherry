using System;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable
{
    public interface ICherryServiceLocatorAndRegistry : IServiceRegistry, IServiceLocator
    {
        object Get(ICherryServiceLocatorAndRegistry originalLocator, Type serviceKey);
    }
}