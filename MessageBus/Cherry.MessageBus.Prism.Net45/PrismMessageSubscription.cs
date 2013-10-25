using System;
using Cherry.MessageBus.Contracts.Portable;
using Microsoft.Practices.Prism.Events;

namespace Cherry.MessageBus.Prism.Net45
{
    internal class PrismMessageSubscription<TMessage> : IMessageSubscription<TMessage>
    {
        private readonly CompositePresentationEvent<TMessage> _evnt;
        private WeakReference<IMessageHandler<TMessage>> _handler;
        private readonly object _disposeLock = new object();
        private bool _hasBeenDisposed;
        private SubscriptionToken _subscriptionToken;

        public PrismMessageSubscription(CompositePresentationEvent<TMessage> evnt, IMessageHandler<TMessage> handler)
        {
            _evnt = evnt;
            ReSubscribe(ThreadOption.PublisherThread);
            _handler = new WeakReference<IMessageHandler<TMessage>>(handler);
        }

        internal void ReSubscribe(ThreadOption threadOption)
        {
            if (_hasBeenDisposed)
            {
                return;
            }
            lock (_disposeLock)
            {
                if (_hasBeenDisposed)
                {
                    return;
                }
                if (_subscriptionToken != null)
                {
                    _evnt.Unsubscribe(_subscriptionToken);
                    _subscriptionToken = null;
                }
                _subscriptionToken = _evnt.Subscribe(OnEventPublished, threadOption, false);
            }
        }

        ~PrismMessageSubscription()
        {
            DisposeInternal();   
        }

        private void OnEventPublished(TMessage message)
        {
            var handler = TryGetHandler();
            if (handler != null)
            {
                handler.Handle(message);
            }
        }

        public bool IsStillSubscribed
        {
            get
            {
                return TryGetHandler() != null;
            }
        }

        public IMessageHandler<TMessage> TryGetHandler()
        {
            if (_hasBeenDisposed)
            {
                return null;
            }
            lock (_disposeLock)
            {
                if (_hasBeenDisposed)
                {
                    return null;
                }
                IMessageHandler<TMessage> handler;
                return _handler.TryGetTarget(out handler) ? handler : null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeInternal();
        }

        private void DisposeInternal()
        {
            if (_hasBeenDisposed)
            {
                return;
            }
            lock (_disposeLock)
            {
                if (_hasBeenDisposed)
                {
                    return;
                }

                _evnt.Unsubscribe(_subscriptionToken);
                _subscriptionToken = null;
                _handler = null;
                _hasBeenDisposed = true;
            }
        }
    }
}