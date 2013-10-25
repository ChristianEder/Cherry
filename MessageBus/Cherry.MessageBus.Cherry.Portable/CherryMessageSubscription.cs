using System;
using Cherry.MessageBus.Contracts.Portable;

namespace Cherry.MessageBus.Cherry.Portable
{
    internal class CherryMessageSubscription<TMessage> : IMessageSubscription<TMessage>
    {
        private CherryMessageChannel<TMessage> _cherryMessageChannel;
        private WeakReference _handler;
        private readonly object _disposeLock = new object();
        private bool _hasBeenDisposed;

        public CherryMessageSubscription(CherryMessageChannel<TMessage> cherryMessageChannel, IMessageHandler<TMessage> handler)
        {
            _cherryMessageChannel = cherryMessageChannel;
            _handler = new WeakReference(handler);
        }

        ~CherryMessageSubscription()
        {
            DisposeInternal();
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
            if (_hasBeenDisposed || !_handler.IsAlive)
            {
                return null;
            }
            lock (_disposeLock)
            {
                if (_hasBeenDisposed || !_handler.IsAlive)
                {
                    return null;
                }
                var handler = _handler.Target as IMessageHandler<TMessage>;
                return handler;
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
                
                _cherryMessageChannel.RemoveFromChannel(this);
                _cherryMessageChannel = null;
                _handler = null;
                _hasBeenDisposed = true;
            }
        }

        internal void Publish(TMessage message)
        {
            var handler = TryGetHandler();
            if (handler == null)
            {
                Dispose();
                return;
            }
            handler.Handle(message);
        }
    }
}