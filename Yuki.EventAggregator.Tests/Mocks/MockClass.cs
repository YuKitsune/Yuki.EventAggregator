using System;
using System.Threading.Tasks;
using Yuki.EventAggregator.Abstractions;
using Yuki.EventAggregator.Attributes;

namespace Yuki.EventAggregator.Tests.Mocks
{
    public class MockClass
    {
        private readonly IEventAggregator _eventAggregator;
        
        public Guid ReceivedMockEventId { get; set; }
        public Guid ReceivedAsyncMockEventId { get; set; }
        public Guid ReceivedAutoMockEventId { get; set; }
        public Guid ReceivedAutoAsyncMockEventId { get; set; }

        public MockClass(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            eventAggregator.Subscribe<MockEvent>(OnMockEvent);
            eventAggregator.Subscribe<MockEvent>(OnMockEventAsync);
        }

        public void OnMockEvent(MockEvent mockEvent)
        {
            ReceivedMockEventId = mockEvent.Guid;
        }
        
        public async Task OnMockEventAsync(MockEvent mockEvent)
        {
            await Task.Yield();
            ReceivedAsyncMockEventId = mockEvent.Guid;
        }
        
        [EventHandler]
        public void AutoOnMockEvent(MockEvent mockEvent)
        {
            ReceivedAutoMockEventId = mockEvent.Guid;
        }

        [EventHandler]
        public async Task AutoOnMockEventAsync(MockEvent mockEvent)
        {
            await Task.Yield();
            ReceivedAutoAsyncMockEventId = mockEvent.Guid;
        }

    }
}