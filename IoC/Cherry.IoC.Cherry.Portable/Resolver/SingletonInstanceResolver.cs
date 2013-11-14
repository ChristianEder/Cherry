using System;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal class SingletonInstanceResolver : IResolver
    {
        private object _instance;

        public SingletonInstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current, InjectionParameter[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                throw new ArgumentException(string.Format("Since \"{0}\" is a singleton instance, you cannot use parameters to resolve it.", _instance), "parameters");
            }
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