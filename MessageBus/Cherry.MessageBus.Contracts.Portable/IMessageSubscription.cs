using System;

namespace Cherry.MessageBus.Contracts.Portable
{
    /// <summary>
    /// Represents a subscription to a specific type of message, and
    /// allows to unsubscribe by disposing this <see cref="IMessageSubscription{TMessage}"/>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that has been subscribed</typeparam>
    public interface IMessageSubscription<in TMessage> : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this <see cref="IMessageSubscription{TMessage}"/>
        /// is still active or if it has been unsubscribed. Since this subscription
        /// only holds a weak reference to the <see cref="IMessageHandler{TMessage}"/>,
        ///  it could also have been unsubscribed by the garbage collector collecting the <see cref="IMessageHandler{TMessage}"/>
        /// </summary>
        bool IsStillSubscribed { get; }

        /// <summary>
        /// Tries to return the handler. If this <see cref="IMessageSubscription{TMessage}"/>
        /// has been unsubscribed (disposed) or the <see cref="IMessageHandler{TMessage}"/>
        /// has been garbage collected, this will return null.
        /// </summary>
        IMessageHandler<TMessage> TryGetHandler();
    }
}
