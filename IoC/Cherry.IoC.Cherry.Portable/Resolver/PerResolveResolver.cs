using System;
using System.Linq;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal class PerResolveResolver : IResolver
    {
        private readonly Type _targetType;

        public PerResolveResolver(Type targetType)
        {
            _targetType = targetType;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current)
        {
            var allPublicConstructors = _targetType.GetConstructors();

            var bestConstructor =
                allPublicConstructors.OrderByDescending(c => c.GetParameters().Count(p => current.CanGet(p.ParameterType)))
                    .FirstOrDefault();

            if (bestConstructor == null)
            {
                throw new InvalidOperationException(string.Format("Creating an instance of type \"{0}\" failed. This type has no public constructors that can be satisfied.", _targetType));
            }

            var parameterInfos = bestConstructor.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                parameters[i] = current.Get(original, parameterInfos[i].ParameterType);
            }

            var instance = bestConstructor.Invoke(parameters);
            return instance;
        }
    }
}