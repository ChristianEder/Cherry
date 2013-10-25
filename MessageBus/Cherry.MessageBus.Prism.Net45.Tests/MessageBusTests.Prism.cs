using Cherry.MessageBus.Contracts.Portable;
using Cherry.MessageBus.Prism.Net45;
using Microsoft.Practices.Prism.Events;

namespace Cherry.MessageBus.Tests
{
    public partial class MessageBusTests
    {
        private IMessageBus CreateMessageBus()
        {
            return new PrismMessageBus(new EventAggregator());
        }

        // TODO: test thread dispatching
    }
}
