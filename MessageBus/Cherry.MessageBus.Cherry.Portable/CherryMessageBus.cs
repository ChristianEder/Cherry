using System;
using System.Collections.Generic;
using Cherry.MessageBus.Contracts.Portable;

namespace Cherry.MessageBus.Cherry.Portable
{
    public class CherryMessageBus : IMessageBus
    {
        private readonly Dictionary<Type, object> _channels = new Dictionary<Type, object>(); 

        public IMessageSubscription<TMessage> Subscribe<TMessage>(IMessageHandler<TMessage> handler)
        {
            var channel = GetChannel<TMessage>();
            return channel.Subscribe(handler);
        }

        public void Publish<TMessage>(TMessage message)
        {
            var channel = GetChannel<TMessage>();
            channel.Publish(message);
        }

        private CherryMessageChannel<TMessage> GetChannel<TMessage>()
        {
            lock (_channels)
            {
                object channel;
                var messageType = typeof (TMessage);
                if (!_channels.TryGetValue(messageType, out channel))
                {
                    channel = new CherryMessageChannel<TMessage>();
                    _channels.Add(messageType, channel);
                }
                return (CherryMessageChannel<TMessage>) channel;
            }
        } 
    }
}
