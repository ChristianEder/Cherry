using Cherry.MessageBus.Cherry.Portable;
using Cherry.MessageBus.Contracts.Portable;

namespace Cherry.MessageBus.Tests
{
    public partial class MessageBusTests
    {
        private IMessageBus CreateMessageBus()
        {
            return new CherryMessageBus();
        }
    }
}
