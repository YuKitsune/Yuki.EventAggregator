using System;
using Yuki.EventAggregator.Abstractions;

namespace Yuki.EventAggregator.Attributes
{
    /// <summary>
    ///     The <see cref="Attribute"/> for automatically specifying a method as an <see cref="IEvent"/> handler for
    ///     the <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventHandlerAttribute : Attribute
    {
    }
}
