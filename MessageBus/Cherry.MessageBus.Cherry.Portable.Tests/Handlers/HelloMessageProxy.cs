using Cherry.MessageBus.Contracts.Portable;
using Cherry.MessageBus.Tests.Messages;

namespace Cherry.MessageBus.Tests.Handlers
{
    public class HelloMessageProxy : IMessageHandler<HelloMessage>
    {
        private readonly IMessageHandler<HelloMessage> _handler;

        public HelloMessageProxy(IMessageHandler<HelloMessage> handler)
        {
            _handler = handler;
        }

        public void Handle(HelloMessage message)
        {
            _handler.Handle(message);
        }
    }
}
