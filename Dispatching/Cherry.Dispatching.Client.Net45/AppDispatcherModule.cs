using Cherry.Dispatching.Contracts.Portable;
using Cherry.IoC.Contracts.Portable;

namespace Cherry.Dispatching.Client.Net45
{
    public class AppDispatcherModule : IModule
    {
        public void Load(IServiceRegistry registry)
        {
            registry.Register<IDispatcher, AppDispatcher>(true);
        }
    }
}