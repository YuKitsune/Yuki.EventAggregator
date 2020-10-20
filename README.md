# About

`Yuki.EventAggregator` provides a lightweight, and easy to use Event Aggregator implementation for .NET Standard 2.1 projects.

# Usage

The `IEventAggregator` is capable of publishing `IEvents`, and invoking their associated handler methods.
Event handler methods are simply `void`, or `async Task` methods, with only one parameter, which must implement the `IEvent` interface.
The default implementation of the `IEventAggregator` is the `InProcessEventAggregator`, which as the name suggests, aggregates events throughout the host process.

## Subscribing to Events

```cs
private readonly IEventAggregator _eventAggregator;
public void Main()
{
    // Create the new Event Aggregator instance
    _eventAggregator = new InProcessEventAggregator();

    // This tells the Event Aggregator to invoke OnSomeEvent whenever SomeEvent is published
    _eventAggregator.Subscribe<SomeEvent>(OnSomeEvent);

    // This does the same as above, but for the async variation
    _eventAggregator.Subscribe<SomeEvent>(OnSomeEventAsync);
}

public void OnSomeEvent(SomeEvent someEvent)
{
    ...
}

public async Task OnSomeEventAsync(SomeEvent someEvent)
{
    ...
}
```

The `IEventAggregator` can also automatically subscribe and unsubscribe any methods with the `EventHandlerAttribute` to their respective events:

```cs
private readonly IEventAggregator _eventAggregator;
public void Main()
{
    // Create the new Event Aggregator instance
    _eventAggregator = new InProcessEventAggregator();
    _eventAggregator.SubscribeAllHandlers(this);
}

[EventHandler]
public void OnSomeEvent(SomeEvent someEvent)
{
    ...
}

[EventHandler]
public async Task OnSomeEventAsync(SomeEvent someEvent)
{
    ...
}

public void Dispose()
{
    _eventAggregator.UnsubscribeAllHandlers(this);
}
```

> ⚠ Important note: If you're using this method, then make sure that UnsubscribeAllHandlers is invoked when the object is no longer being used. Implementing the `IDisposable` interface should make this easier to work with.

## Publishing Events
```cs
// This will publish a new instance of SomeEvent, any synchronous event handler which is subscribed to the SomeEvent event will be invoked with the SomeEvent as it's parameter
_eventAggregator.Publish(new SomeEvent("Dummy string"));

// This will do the same as above, but will run asynchronously, and only async event handlers will be invoked
await _eventAggregator.PublishAsync(new SomeEvent("Dummy string"));
```

# Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`feature/AmazingFeature`)
3. Commit your Changes
4. Push to the Branch
5. Open a Pull Request
