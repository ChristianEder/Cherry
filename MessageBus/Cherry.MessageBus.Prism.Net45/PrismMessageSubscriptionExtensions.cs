using System;
using Cherry.MessageBus.Contracts.Portable;
using Microsoft.Practices.Prism.Events;

namespace Cherry.MessageBus.Prism.Net45
{
    public static class PrismMessageSubscriptionExtensions
    {
        public static IMessageSubscription<TMessage> OnUIThread<TMessage>(this IMessageSubscription<TMessage> subscription)
        {
            return DispatchTo(subscription, ThreadOption.UIThread);
        }

        public static IMessageSubscription<TMessage> OnPublisherThread<TMessage>(this IMessageSubscription<TMessage> subscription)
        {
            return DispatchTo(subscription, ThreadOption.PublisherThread);
        }

        public static IMessageSubscription<TMessage> OnBackgroundThread<TMessage>(this IMessageSubscription<TMessage> subscription)
        {
            return DispatchTo(subscription, ThreadOption.BackgroundThread);
        }

        private static IMessageSubscription<TMessage> DispatchTo<TMessage>(IMessageSubscription<TMessage> subscription, ThreadOption threadOption)
        {
            if (ReferenceEquals(subscription, null))
            {
                throw new ArgumentNullException("subscription", "subscription must not be null");
            }
            var prismSubscription = subscription as PrismMessageSubscription<TMessage>;
            if (ReferenceEquals(prismSubscription, null))
            {
                throw new ArgumentNullException("subscription", string.Format("{0} can only work with {1} instances of type {2}", typeof(PrismMessageSubscriptionExtensions), typeof(IMessageSubscription<>), typeof(PrismMessageSubscription<>)));
            }

            prismSubscription.ReSubscribe(threadOption);
            return prismSubscription;
        }
    }
}