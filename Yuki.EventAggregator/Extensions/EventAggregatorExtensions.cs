using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yuki.EventAggregator.Abstractions;
using Yuki.EventAggregator.Attributes;

namespace Yuki.EventAggregator.Extensions
{
    /// <summary>
    ///     The <see cref="IEventAggregator"/> extension methods.
    /// </summary>
    public static class EventAggregatorExtensions
    {
        /// <summary>
        ///     Automatically subscribes all methods with the <see cref="EventHandlerAttribute"/> to
        ///     <see cref="IEvent"/>s published over the <paramref name="eventAggregator"/>.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/> to subscribe the methods to.
        /// </param>
        /// <param name="targetObject">
        ///     The <see cref="object"/> containing the <see cref="IEvent"/> handlers.
        /// </param>
        public static void SubscribeAllHandlers(this IEventAggregator eventAggregator, object targetObject)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

            // Get all handlers
            List<object> handlers = GetHandlers(targetObject).ToList();

            // Subscribe them all
            foreach (object handler in handlers)
            {
                MethodInfo subscribeMethod = GetSubscriberMethodFor(eventAggregator, handler.GetType());
                subscribeMethod.Invoke(eventAggregator, new[] { handler });
            }
        }

        /// <summary>
        ///     Automatically unsubscribes all methods with the <see cref="EventHandlerAttribute"/> from
        ///     <see cref="IEvent"/>s published over the <paramref name="eventAggregator"/>.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/> to unsubscribe the methods from.
        /// </param>
        /// <param name="targetObject">
        ///     The <see cref="object"/> containing the <see cref="IEvent"/> handlers.
        /// </param>
        public static void UnsubscribeAllHandlers(this IEventAggregator eventAggregator, object targetObject)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

            // Get all handlers
            List<object> handlers = GetHandlers(targetObject).ToList();

            // Unsubscribe them all
            foreach (object handler in handlers)
            {
                MethodInfo unsubscribeMethod = GetUnSubscriberMethodFor(eventAggregator, handler.GetType());
                unsubscribeMethod.Invoke(eventAggregator, new[] { handler });
            }
        }

        /// <summary>
        ///     Gets all methods on the <paramref name="targetObject"/> with the <see cref="EventHandlerAttribute"/>
        ///     which are eligible for handling <see cref="IEvent"/>s.
        /// </summary>
        /// <param name="targetObject">
        ///     The <see cref="object"/> containing the <see cref="IEvent"/> handlers.
        /// </param>
        /// <returns>
        ///     The <see cref="ICollection{T}"/> of <see cref="object"/>s which are either <see cref="Action{T}"/>s or
        ///     <see cref="Func{T, TResult}"/>s which can handle <see cref="IEvent"/>s.
        /// </returns>
        private static ICollection<object> GetHandlers(object targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));
            MethodInfo[] methods = targetObject.GetType()
                                               .GetMethods()
                                               .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() != null)
                                               .ToArray();

            List<object> delegates = new List<object>();
            foreach (MethodInfo method in methods)
            {
                object @delegate = GetDelegateFromMethod(method, targetObject);
                delegates.Add(@delegate);
            }

            return delegates;
        }

        /// <summary>
        ///     Gets the <see cref="object"/> representing the <see cref="Action{T}"/> or
        ///     <see cref="Func{T, TResult}"/> derived from the <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">
        ///     The <see cref="MethodInfo"/> to get the delegate from.
        /// </param>
        /// <param name="targetObject">
        ///     The <see cref="object"/> where the <see cref="method"/> can be found.
        /// </param>
        /// <returns>
        ///     The <see cref="object"/> representing the <see cref="Action{T}"/> or <see cref="Func{T, TResult}"/>.
        /// </returns>
        private static object GetDelegateFromMethod(MethodInfo method, object targetObject)
        {
            // Make sure the method only has one parameter, and it's an IEvent
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 1)
            {
                throw new NotSupportedException($"The method \"{method.Name}\" has {parameters.Length} parameters. Only 1 parameter of type {nameof(IEvent)} is supported.");
            }

            ParameterInfo parameter = parameters.First();
            Type parameterType = parameter.ParameterType;
            if (!typeof(IEvent).IsAssignableFrom(parameterType))
            {
                throw new NotSupportedException($"The method \"{method.Name}\" must have a parameter of type {nameof(IEvent)}.");
            }

            // Make our type using our parameter and return type
            Type delegateType;
            if (method.ReturnType == typeof(void))
            {
                // Void method makes an Action<T>
                delegateType = typeof(Action<>).MakeGenericType(parameterType);
            }
            else if (method.ReturnType == typeof(Task))
            {
                // Task makes a Func<T, Task>
                delegateType = typeof(Func<,>).MakeGenericType(parameterType, typeof(Task));
            }
            else
            {
                throw new NotSupportedException($"The type \"{method.ReturnType.Name}\" is not supported for use as an {nameof(IEvent)} handler.");
            }

            // Create our delegate
            return method.CreateDelegate(delegateType, targetObject);
        }

        /// <summary>
        ///     Gets the Subscribe <see cref="MethodInfo"/> from the given <see cref="IEventAggregator"/> for the
        ///     given <see cref="IEvent"/> handler <see cref="Type"/>.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/>.
        /// </param>
        /// <param name="handlerType">
        ///     The <see cref="Type"/> of the <see cref="IEvent"/> handler.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/> for the required subscribe method.
        /// </returns>
        private static MethodInfo GetSubscriberMethodFor(IEventAggregator eventAggregator, Type handlerType) =>
            GetSubscriptionMethod(eventAggregator, nameof(IEventAggregator.Subscribe), handlerType);

        /// <summary>
        ///     Gets the Unsubscribe <see cref="MethodInfo"/> from the given <see cref="IEventAggregator"/> for the
        ///     given <see cref="IEvent"/> handler <see cref="Type"/>.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/>.
        /// </param>
        /// <param name="handlerType">
        ///     The <see cref="Type"/> of the <see cref="IEvent"/> handler.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/> for the required unsubscribe method.
        /// </returns>
        private static MethodInfo GetUnSubscriberMethodFor(IEventAggregator eventAggregator, Type handlerType) =>
            GetSubscriptionMethod(eventAggregator, nameof(IEventAggregator.Unsubscribe), handlerType);

        /// <summary>
        ///     Gets the <see cref="IEventAggregator"/>s Subscribe or Unsubscribe method for the specific
        ///     <see cref="handlerType"/>.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/>.
        /// </param>
        /// <param name="methodName">
        ///     The name of the method to search for.
        /// </param>
        /// <param name="handlerType">
        ///     The <see cref="Type"/> of the <see cref="IEvent"/> handler.
        /// </param>
        /// <returns>
        ///     The <see cref="MethodInfo"/> for the subscribe or unsubscribe method.
        /// </returns>
        private static MethodInfo GetSubscriptionMethod(
            IEventAggregator eventAggregator,
            string methodName,
            Type handlerType)
        {
            // Get the type of IEvent we're dealing with
            Type eventType = handlerType.GenericTypeArguments.First();

            // Get all possible methods we could have
            MethodInfo[] possibleMethods = eventAggregator.GetType().GetMethods().Where(m => m.Name == methodName).ToArray();
            foreach (MethodInfo method in possibleMethods)
            {
                // Make sure amount of generic parameters in handler match
                Type[] delegateParameters = method.GetParameters().First().ParameterType.GenericTypeArguments;
                if (delegateParameters.Length != handlerType.GetGenericArguments().Length)
                {
                    continue;
                }

                // Make sure the first parameter is generic
                Type firstParameter = delegateParameters.First();
                if (!firstParameter.IsGenericParameter)
                {
                    continue;
                }

                // Found our method
                return method.MakeGenericMethod(eventType);
            }

            // Nothing found
            throw new Exception($"Couldn't find any method named {methodName} which deals with methods with parameters {string.Join(", ", handlerType.GenericTypeArguments.Select(t => t.Name))}.");
        }
    }
}
