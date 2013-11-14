using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Cherry.Portable.Resolver
{
    internal class PerResolveResolver : IResolver
    {
        private readonly Type _targetType;

        public PerResolveResolver(Type targetType)
        {
            _targetType = targetType;
        }

        public object Get(ICherryServiceLocatorAndRegistry original, ICherryServiceLocatorAndRegistry current,
            InjectionParameter[] parameters)
        {
            ConstructorInfo[] allPublicConstructors = _targetType.GetConstructors();

            ConstructorInfo bestConstructor =
                allPublicConstructors.OrderByDescending(
                    c => c.GetParameters().Count(p => CanResolveParameter(current, p, parameters)))
                    .FirstOrDefault();

            if (bestConstructor == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Creating an instance of type \"{0}\" failed. This type has no public constructors that can be satisfied.",
                        _targetType));
            }

            ParameterInfo[] constructorParameterInfos = bestConstructor.GetParameters();

            ValidateParameters(constructorParameterInfos, parameters);

            var constructorParameters = new object[constructorParameterInfos.Length];
            for (int i = 0; i < constructorParameterInfos.Length; i++)
            {
                constructorParameters[i] = ResolveParameter(original, current, constructorParameterInfos, i, parameters);
            }

            object instance = bestConstructor.Invoke(constructorParameters);
            return instance;
        }

        public void Dispose()
        {
        }

        private void ValidateParameters(IEnumerable<ParameterInfo> constructorParameterInfos, InjectionParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return;
            }

            var namedParametersWithoutMatchingConstructorParameter =
                parameters.Where(
                    p => !string.IsNullOrEmpty(p.Key) && constructorParameterInfos.All(cp => cp.Name != p.Key));
            if (namedParametersWithoutMatchingConstructorParameter.Any())
            {
                throw new ArgumentException("The provided parameters contain parameter names that cannot be matched to the constructor of type " + _targetType, "parameters");
            }

            var unnamedParametersByType = parameters
                .Where(p => string.IsNullOrEmpty(p.Key) &&
                            !ReferenceEquals(p.Value, null))
                .GroupBy(p => p.Value.GetType())
                .Select(p => new {Type = p.Key, Count = p.Count()})
                .ToArray();

            if (unnamedParametersByType.Any(p => p.Count > 1))
            {
                throw new ArgumentException("The provided parameters contain multiple unnamed parameters with the same type", "parameters");
            }

            if (
                unnamedParametersByType.Any(
                    p => constructorParameterInfos.Count(cp => cp.ParameterType.IsAssignableFrom(p.Type)) > 1))
            {
                throw new ArgumentException("The provided parameters contain unnamed parameters that cannot be mapped unambiguously to the constructor of type " + _targetType, "parameters");
            }
        }

        private static bool CanResolveParameter(ICherryServiceLocatorAndRegistry current,
            ParameterInfo constructorParameterInfo, InjectionParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                // Shotcut when no parameters provided
                return current.CanGet(constructorParameterInfo.ParameterType);
            }

            InjectionParameter namedParameter = parameters.SingleOrDefault(p => p.Key == constructorParameterInfo.Name);
            if (namedParameter != null)
            {
                return true;
            }

            InjectionParameter unnamedParameter = parameters.SingleOrDefault(
                p => string.IsNullOrEmpty(p.Key) &&
                     !ReferenceEquals(p.Value, null)
                     && constructorParameterInfo.ParameterType.IsInstanceOfType(p.Value));
            if (unnamedParameter != null)
            {
                return true;
            }

            return current.CanGet(constructorParameterInfo.ParameterType);
        }

        private static object ResolveParameter(ICherryServiceLocatorAndRegistry original,
            ICherryServiceLocatorAndRegistry current, ParameterInfo[] constructorParameterInfos, int i,
            InjectionParameter[] parameters)
        {
            ParameterInfo constructorParameterInfo = constructorParameterInfos[i];

            if (parameters == null || parameters.Length == 0)
            {
                // Shotcut when no parameters provided
                return current.Get(original, constructorParameterInfo.ParameterType);
            }


            InjectionParameter namedParameter = parameters.SingleOrDefault(p => p.Key == constructorParameterInfo.Name);
            if (namedParameter != null)
            {
                return namedParameter.Value;
            }

            // No matching named parameter found, so lets try if we have a single parameter that mathes the constructor parameter type
            InjectionParameter unnamedParameter = parameters.SingleOrDefault(
                p => string.IsNullOrEmpty(p.Key) &&
                     !ReferenceEquals(p.Value, null)
                     && constructorParameterInfo.ParameterType.IsInstanceOfType(p.Value));

            if (unnamedParameter != null)
            {
                return unnamedParameter.Value;
            }

            return current.Get(original, constructorParameterInfo.ParameterType);
        }
    }
}