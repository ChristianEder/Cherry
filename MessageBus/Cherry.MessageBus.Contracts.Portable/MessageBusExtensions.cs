using System;

namespace Cherry.MessageBus.Contracts.Portable
{
    /// <summary>
    /// Provides extension methods for the <see cref="IMessageBus"/> interface
    /// </summary>
    public static class MessageBusExtensions
    {
        private class ActionMessageHandler<TMessage> : IMessageHandler<TMessage>
        {
            private readonly Action<TMessage> _handler;

            public ActionMessageHandler(Action<TMessage> handler)
            {
                _handler = handler;
            }

            public void Handle(TMessage message)
            {
                _handler(message);
            }
        }

        /// <summary>
        /// Subscribes the <paramref name="handler" /> to
        /// all messages of type <typeparamref name="TMessage" />
        /// </summary>
        /// <typeparam name="TMessage">The type of message the handler is interested in</typeparam>
        /// <param name="messageBus">The message bus to subscribe on</param>
        /// <param name="handler">The handler that is interested in messages</param>
        /// <returns>
        /// A subscription that allows the caller to unsubscribe by disposing it
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// messageBus;The messageBus must not be null
        /// or
        /// handler;The handler must not be null
        /// </exception>
        public static IMessageSubscription<TMessage> Subscribe<TMessage>(this IMessageBus messageBus, Action<TMessage> handler)
        {
            if (ReferenceEquals(messageBus, null))
            {
                throw new ArgumentNullException("messageBus", "The messageBus must not be null");
            }
            if (ReferenceEquals(handler, null))
            {
                throw new ArgumentNullException("handler", "The handler must not be null");
            }
            return messageBus.Subscribe(new ActionMessageHandler<TMessage>(handler));
        } 
    }
}