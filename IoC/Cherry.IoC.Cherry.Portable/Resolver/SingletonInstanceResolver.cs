using System;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    public class SingletonInstanceResolver : IResolver
    {
        private object _instance;

        public SingletonInstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current)
        {
            return _instance;
        }

        public void Dispose()
        {
            var disposable = _instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            _instance = null;
        }
    }
}