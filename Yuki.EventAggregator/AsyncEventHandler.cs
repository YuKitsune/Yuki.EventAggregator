using System;
using System.Threading.Tasks;
using Yuki.EventAggregator.Abstractions;

namespace Yuki.EventAggregator
{
    /// <summary>
    ///     The class containing an asynchronous handler for a specific <see cref="IEvent"/>.
    /// </summary>
    internal sealed class AsyncEventHandler
    {
        /// <summary>
        ///     Gets the <see cref="Type"/> of <see cref="IEvent"/> being handled.
        /// </summary>
        internal Type EventType { get; }

        /// <summary>
        ///     Gets the <see cref="object"/> handling the <see cref="IEvent"/>.
        ///     This is assumed to be an <see cref="Func{T, TResult}"/> where the result is a <see cref="Task"/>.
        /// </summary>
        internal object Handler { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncEventHandler"/> class.
        /// </summary>
        /// <param name="eventType">
        ///     The <see cref="Type"/> of the <see cref="IEvent"/> being handled.
        /// </param>
        /// <param name="handler">
        ///     The <see cref="Func{T, TResult}"/> handling the <see cref="IEvent"/> as an asynchronous operation.
        /// </param>
        internal AsyncEventHandler(Type eventType, object handler)
        {
            // Ensure the eventType is an event
            if (!typeof(IEvent).IsAssignableFrom(eventType))
            {
                throw new ArgumentException($"The {nameof(eventType)} does not implement {nameof(IEvent)}.", nameof(eventType));
            }

            // Ensure the Handler is of the correct type
            if (handler.GetType() != GetHandlerType(eventType))
            {
                throw new ArgumentException($"The {nameof(handler)} is not appropriate for handling the event type \"{eventType.Name}\".", nameof(handler));
            }

            EventType = eventType;
            Handler = handler;
        }

        /// <summary>
        ///     Handles the <typeparamref name="TEvent"/> as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/> to handle.
        /// </typeparam>
        /// <param name="event">
        ///     The <typeparamref name="TEvent"/> to handle.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        internal async Task HandleAsync<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            // Ensure the event type is correct
            Type eventType = typeof(TEvent);
            if (eventType != EventType)
            {
                throw new NotSupportedException($"The handler for \"{EventType.Name}\" does not support handling \"{eventType.Name}\".");
            }

            await ((Func<TEvent, Task>)Handler).Invoke(@event);
        }

        /// <summary>
        ///     Gets the expected <see cref="Type"/> of the <see cref="Handler"/> given the <see cref="IEvent"/>s
        ///     <see cref="Type"/>.
        /// </summary>
        /// <param name="eventType">
        ///     The <see cref="Type"/> of <see cref="IEvent"/> to use as the parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="Type"/> of handler.
        /// </returns>
        private Type GetHandlerType(Type eventType)
        {
            Type funcType = typeof(Func<,>);
            return funcType.MakeGenericType(eventType, typeof(Task));
        }
    }
}
