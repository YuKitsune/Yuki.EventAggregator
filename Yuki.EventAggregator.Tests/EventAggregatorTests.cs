using System;
using System.Threading.Tasks;

using NUnit.Framework;

using Yuki.EventAggregator.Abstractions;
using Yuki.EventAggregator.Extensions;
using Yuki.EventAggregator.Tests.Mocks;

namespace Yuki.EventAggregator.Tests
{
    /// <summary>
    ///     The <see cref="InProcessEventAggregator"/> tests.
    /// </summary>
    [TestFixture]
    public class EventAggregatorTests
    {
        /// <summary>
        ///     Ensures <see cref="IEvent"/>s can be published over the <see cref="InProcessEventAggregator"/>, and subscribers
        ///     can handle the <see cref="IEvent"/>s.
        /// </summary>
        [Test]
        public void EventsCanBePublished()
        {
            // Arrange
            IEventAggregator eventAggregator = new InProcessEventAggregator();
            MockClass mockClass = new MockClass(eventAggregator);
            MockEvent @event = new MockEvent();

            // Act
            eventAggregator.Publish(@event);

            // Assert
            Assert.IsNotNull(mockClass.ReceivedMockEventId);
            Assert.AreEqual(@event.Guid, mockClass.ReceivedMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAsyncMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoAsyncMockEventId);
        }

        /// <summary>
        ///     Ensures <see cref="IEvent"/>s can be published asynchronously over the <see cref="InProcessEventAggregator"/>,
        ///     and subscribers can handle the <see cref="IEvent"/>s.
        /// </summary>
        [Test]
        public async Task EventsCanBePublishedAsync()
        {
            // Arrange
            IEventAggregator eventAggregator = new InProcessEventAggregator();
            MockClass mockClass = new MockClass(eventAggregator);
            MockEvent @event = new MockEvent();

            // Act
            await eventAggregator.PublishAsync(@event);

            // Assert
            Assert.IsNotNull(mockClass.ReceivedAsyncMockEventId);
            Assert.AreEqual(@event.Guid, mockClass.ReceivedAsyncMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoAsyncMockEventId);
        }

        /// <summary>
        ///     Ensures the <see cref="IEventAggregator"/> can automatically subscribe all available Event Handlers.
        /// </summary>
        [Test]
        public async Task AutoSubscribeWorks()
        {
            // Arrange
            IEventAggregator eventAggregator = new InProcessEventAggregator();
            MockClass mockClass = new MockClass(eventAggregator);
            eventAggregator.SubscribeAllHandlers(mockClass);
            MockEvent @event = new MockEvent();

            // Act
            // ReSharper disable once MethodHasAsyncOverload
            //     ^ Actually want to test non-async method as well
            eventAggregator.Publish(@event);
            await eventAggregator.PublishAsync(@event);

            // Assert
            Assert.IsNotNull(mockClass.ReceivedAutoMockEventId);
            Assert.IsNotNull(mockClass.ReceivedAutoAsyncMockEventId);
            Assert.AreEqual(@event.Guid, mockClass.ReceivedAutoMockEventId);
            Assert.AreEqual(@event.Guid, mockClass.ReceivedAutoAsyncMockEventId);
        }

        /// <summary>
        ///     Ensures the <see cref="IEventAggregator"/> can automatically unsubscribe from all available Event
        ///     Handlers.
        /// </summary>
        [Test]
        public async Task AutoUnSubscribeWorks()
        {
            // Arrange
            IEventAggregator eventAggregator = new InProcessEventAggregator();
            MockClass mockClass = new MockClass(eventAggregator);
            eventAggregator.SubscribeAllHandlers(mockClass);
            eventAggregator.UnsubscribeAllHandlers(mockClass);
            MockEvent @event = new MockEvent();

            // Act
            // ReSharper disable once MethodHasAsyncOverload
            //     ^ Actually want to test non-async method as well
            eventAggregator.Publish(@event);
            await eventAggregator.PublishAsync(@event);

            // Assert
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoMockEventId);
            Assert.AreEqual(default(Guid), mockClass.ReceivedAutoAsyncMockEventId);
        }
    }
}
