using System;
using System.Linq;
using System.Reflection;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.IoC.Common.Portable
{
    public static class ServiceLocatorFactoryMethodSupportExtensions
    {
        private static readonly Type FuncType;
        private static readonly MethodInfo GetMethod;

        static ServiceLocatorFactoryMethodSupportExtensions()
        {
            FuncType = typeof (Func<>);
            GetMethod = typeof(ServiceLocatorFactoryMethodSupportExtensions).GetMethod("CreateFactoryMethod", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static bool IsFactoryMethod(Type resolvedType, out Type factoryMethodType)
        {
            if (ReferenceEquals(resolvedType, null))
            {
                factoryMethodType = null;
                return false;
            }
            if (resolvedType.IsGenericType && resolvedType.GetGenericTypeDefinition() == FuncType)
            {
                factoryMethodType = resolvedType.GetGenericArguments().Single();
                return true;
            }
            factoryMethodType = null;
            return false;
        }

        public static bool GetFactoryMethod(IServiceLocator locator, Type resolveType, out object factoryMethod)
        {
            Type factoryMethodType;
            if (!IsFactoryMethod(resolveType, out factoryMethodType))
            {
                factoryMethod = null;
                return false;
            }

            var genericMethod = GetMethod.MakeGenericMethod(factoryMethodType);

            factoryMethod = genericMethod.Invoke(null, new object[] { locator });
            return true;
        }

        private static Func<T> CreateFactoryMethod<T>(IServiceLocator locator)
        {
            return () => locator.Get<T>();
        } 
    }
}
