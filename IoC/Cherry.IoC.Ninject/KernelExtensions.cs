using System;
using System.Linq;
using Ninject;

namespace Cherry.IoC.Ninject
{
    public static class KernelExtensions
    {
        public static bool IsBound(this IKernel kernel, Type serviceKey)
        {
            return kernel.GetBindings(serviceKey).Any();
        }
    }
}