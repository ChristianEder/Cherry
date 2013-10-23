namespace Cherry.MessageBus.Contracts.Portable
{
    /// <summary>
    /// The <see cref="IMessageBus"/> allows loosely coupled
    /// communication between different parts of the system 
    /// using a message / event style communication
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Subscribes the <paramref name="handler" /> to
        /// all messages of type <typeparamref name="TMessage" />
        /// </summary>
        /// <typeparam name="TMessage">The type of message the handler is interested in</typeparam>
        /// <param name="handler">The handler that is interested in messages</param>
        /// <returns>
        /// A subscription that allows the caller to unsubscribe by disposing it
        /// </returns>
        IMessageSubscription<TMessage> Subscribe<TMessage>(IMessageHandler<TMessage> handler);

        /// <summary>
        /// Publishes a message. All <see cref="IMessageHandler{TMessage}"/> that have
        /// <see cref="Subscribe{TMessage}"/>d previously, will get the message
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be published</typeparam>
        /// <param name="message">The message to be published</param>
        void Publish<TMessage>(TMessage message);
    }
}