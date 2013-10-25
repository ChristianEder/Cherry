using Cherry.MessageBus.Contracts.Portable;
using Microsoft.Practices.Prism.Events;

namespace Cherry.MessageBus.Prism.Net45
{
    public class PrismMessageBus : IMessageBus
    {
        private readonly IEventAggregator _eventAggregator;

        public PrismMessageBus(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IMessageSubscription<TMessage> Subscribe<TMessage>(IMessageHandler<TMessage> handler)
        {
            var compositePresentationEvent = _eventAggregator.GetEvent<CompositePresentationEvent<TMessage>>();
            return new PrismMessageSubscription<TMessage>(compositePresentationEvent, handler);
        }

        public void Publish<TMessage>(TMessage message)
        {
            var compositePresentationEvent = _eventAggregator.GetEvent<CompositePresentationEvent<TMessage>>();
            compositePresentationEvent.Publish(message);
        }
    }
}
