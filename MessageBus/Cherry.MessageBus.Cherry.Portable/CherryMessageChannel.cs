using System;
using System.Collections.Generic;
using Cherry.MessageBus.Contracts.Portable;

namespace Cherry.MessageBus.Cherry.Portable
{
    internal class CherryMessageChannel<TMessage>
    {
        private readonly List<CherryMessageSubscription<TMessage>> _subscriptions = new List<CherryMessageSubscription<TMessage>>();

        internal IMessageSubscription<TMessage> Subscribe(IMessageHandler<TMessage> handler)
        {
            lock (_subscriptions)
            {
                var subscription = new CherryMessageSubscription<TMessage>(this, handler);
                _subscriptions.Add(subscription);
                return subscription;
            }
        }

        internal void Publish(TMessage message)
        {
            CherryMessageSubscription<TMessage>[] subscriptions;
            lock (_subscriptions)
            {
                subscriptions = _subscriptions.ToArray();
            }
            foreach (var subscription in subscriptions)
            {
                subscription.Publish(message);
            }
        }

        internal void RemoveFromChannel(CherryMessageSubscription<TMessage> subscription)
        {
            lock (_subscriptions)
            {
                _subscriptions.Remove(subscription);
            }
        }
    }
}