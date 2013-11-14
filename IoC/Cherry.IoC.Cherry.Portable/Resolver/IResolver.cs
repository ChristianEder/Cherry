using System;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal interface IResolver : IDisposable
    {
        object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current, InjectionParameter[] parameters);
    }
}