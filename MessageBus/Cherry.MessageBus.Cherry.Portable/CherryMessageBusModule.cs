using Cherry.IoC.Contracts.Portable;
using Cherry.MessageBus.Contracts.Portable;

namespace Cherry.MessageBus.Cherry.Portable
{
    public class CherryMessageBusModule : IModule
    {
        public void Load(IServiceRegistry registry)
        {
            registry.Register<IMessageBus, CherryMessageBus>(true);
        }
    }
}