using System;
using Yuki.EventAggregator.Abstractions;

namespace Yuki.EventAggregator.Tests.Mocks
{
    public class MockEvent : IEvent
    {
        public Guid Guid { get; }
        public MockEvent() => Guid = Guid.NewGuid();
    }
}