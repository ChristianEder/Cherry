using System;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal class SingletonResolver : IResolver
    {
        private readonly Type _targetType;
        private volatile object _instance;
        private readonly object _syncRoot = new object();

        public SingletonResolver(Type targetType)
        {
            _targetType = targetType;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current, InjectionParameter[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                throw new ArgumentException(string.Format("Since \"{0}\" is a singleton instance, you cannot use parameters to resolve it.", _instance), "parameters");
            }

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
                var instance = (new PerResolveResolver(_targetType)).Get(original, current, parameters);
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