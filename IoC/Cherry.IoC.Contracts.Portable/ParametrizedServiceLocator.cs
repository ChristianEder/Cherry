using System;
using System.Collections.Generic;
using System.Linq;

namespace Cherry.IoC.Contracts.Portable
{
    internal class ParametrizedServiceLocator : IParametrizedServiceLocator
    {
        private readonly IServiceLocator _locator;
        private readonly List<InjectionParameter> _parameters = new List<InjectionParameter>();

        public ParametrizedServiceLocator(IServiceLocator locator)
        {
            _locator = locator;
        }

        public IParametrizedServiceLocator With<TValue>(TValue value)
        {
            _parameters.Add(new InjectionParameter(null, value));
            return this;
        }

        public IParametrizedServiceLocator With<TValue>(string key, TValue value)
        {
            _parameters.Add(new InjectionParameter(key, value));
            return this;
        }

        public object Get(Type serviceKey, params InjectionParameter[] parameters)
        {
            IEnumerable<InjectionParameter> ps = _parameters;
            if (parameters != null && parameters.Length > 0)
            {
                ps = ps.Concat(parameters);
            }
            return _locator.Get(serviceKey, ps.ToArray());
        }

        public bool CanGet(Type serviceKey)
        {
            return _locator.CanGet(serviceKey);
        }
    }
}