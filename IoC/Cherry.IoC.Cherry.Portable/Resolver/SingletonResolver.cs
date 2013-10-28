using System;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    public class SingletonResolver : IResolver
    {
        private readonly Type _targetType;
        private volatile object _instance;
        private readonly object _syncRoot = new object();

        public SingletonResolver(Type targetType)
        {
            _targetType = targetType;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current)
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_syncRoot)
            {
                if (_instance != null)
                {
                    return _instance;
                }
                var instance = (new PerResolveResolver(_targetType)).Get(original, current);
                _instance = instance;
                return instance;
            }
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