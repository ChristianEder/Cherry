using System;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    public interface IResolver : IDisposable
    {
        object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current);
    }
}