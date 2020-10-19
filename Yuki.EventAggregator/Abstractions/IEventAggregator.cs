using System;
using System.Threading.Tasks;

namespace Yuki.EventAggregator.Abstractions
{
    /// <summary>
    ///     The interface representing a service that provides the ability to publish an <see cref="IEvent"/> from one
    ///     entity to another in a loosely based fashion.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        ///     Subscribes the given <see cref="Action{T}"/> to the all published <see cref="IEvent"/>s of type
        ///     <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Action{T}"/> handling the <typeparamref name="TEvent"/>.
        /// </param>
        void Subscribe<TEvent>(Action<TEvent> handlerAction)
            where TEvent : IEvent;

        /// <summary>
        ///     Subscribes the given <see cref="Func{T, Task}"/> to the all published <see cref="IEvent"/>s of type
        ///     <typeparamref name="TEvent"/> which were published asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Func{T, Task}"/> handling the <typeparamref name="TEvent"/> as an asynchronous
        ///     operation.
        /// </param>
        void Subscribe<TEvent>(Func<TEvent, Task> handlerAction)
            where TEvent : IEvent;

        /// <summary>
        ///     Gets a value indicating whether or not the given <paramref name="handlerAction"/>  is subscribed to any
        ///     <see cref="IEvent"/>s.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Func{T, Task}"/> which may be handling the <typeparamref name="TEvent"/> as an
        ///     asynchronous operation.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="handlerAction"/> is subscribed to any <see cref="IEvent"/>.
        /// </returns>
        bool IsSubscribed<TEvent>(Action<TEvent> handlerAction)
            where TEvent : IEvent;

        /// <summary>
        ///     Gets a value indicating whether or not the given <paramref name="handlerAction"/>  is subscribed to any
        ///     <see cref="IEvent"/>s.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Func{T, Task}"/> which may be handling the <typeparamref name="TEvent"/> as an
        ///     asynchronous operation.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="handlerAction"/> is subscribed to any <see cref="IEvent"/>.
        /// </returns>
        bool IsSubscribed<TEvent>(Func<TEvent, Task> handlerAction)
            where TEvent : IEvent;

        /// <summary>
        ///     Publishes the <typeparamref name="TEvent"/> to for all subscribers to handle.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="event">
        ///     The <typeparamref name="TEvent"/> to publish.
        /// </param>
        void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;

        /// <summary>
        ///     Publishes the <typeparamref name="TEvent"/> to for all subscribers to handle as an asynchronous
        ///     operation.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="event">
        ///     The <typeparamref name="TEvent"/> to publish.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;

        /// <summary>
        ///     Unsubscribes the given <see cref="Action{T}"/> from all published <see cref="IEvent"/>s of type
        ///     <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Action{T}"/> handling the <typeparamref name="TEvent"/>.
        /// </param>
        void Unsubscribe<TEvent>(Action<TEvent> handlerAction)
            where TEvent : IEvent;

        /// <summary>
        ///     Unsubscribes the given <see cref="Func{T, Task}"/> from all published <see cref="IEvent"/>s of type
        ///     <typeparamref name="TEvent"/> which were published asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">
        ///     The type of <see cref="IEvent"/>.
        /// </typeparam>
        /// <param name="handlerAction">
        ///     The <see cref="Func{T, Task}"/> handling the <typeparamref name="TEvent"/> as an asynchronous
        ///     operation.
        /// </param>
        void Unsubscribe<TEvent>(Func<TEvent, Task> handlerAction)
            where TEvent : IEvent;
    }
}
