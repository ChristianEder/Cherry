namespace Cherry.MessageBus.Contracts.Portable
{
    /// <summary>
    /// Represents a handler of a specific type of message
    /// </summary>
    /// <typeparam name="TMessage">The type of message to be handled</typeparam>
    public interface IMessageHandler<in TMessage>
    {
        /// <summary>
        /// Gets called whenever a message of type <typeparamref name="TMessage"/>
        /// has been published using the <see cref="IMessageBus"/>
        /// </summary>
        /// <param name="message">The message to be handled</param>
        void Handle(TMessage message);
    }
}