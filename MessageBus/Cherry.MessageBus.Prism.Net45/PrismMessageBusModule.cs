using Cherry.IoC.Contracts.Portable;
using Cherry.MessageBus.Contracts.Portable;
using Microsoft.Practices.Prism.Events;

namespace Cherry.MessageBus.Prism.Net45
{
    public class PrismMessageBusModule : IModule
    {
        public void Load(IServiceRegistry registry)
        {
            if (!registry.IsRegistered(typeof (IEventAggregator)))
            {
                registry.Register<IEventAggregator, EventAggregator>(true);
            }
            registry.Register<IMessageBus, PrismMessageBus>(true);
        }
    }
}