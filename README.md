Nerve
=====

![Nerve logo](http://i.kostassoid.com/nerve/logo.png)

Nerve is a federated message bus for .NET. It allows for fast and easy concurrent message routing and processing without thread synchronization.

To name a few:
* [Event Aggregator](http://martinfowler.com/eaaDev/EventAggregator.html)
* [Actor Model](http://en.wikipedia.org/wiki/Actor_model)
* [Reactive Extensions](https://rx.codeplex.com/)
* [Apache Camel](https://camel.apache.org/)

Installation
------------

The easiest way is to use NuGet:

    Install-Package Nerve-Core

Basic usage
-----------

A basic building block of Nerve infrastructure is a Cell, which, by default implementation, is basically an in-proc queue with broadcast routing. All processing within a Cell is serialized but can be scheduled on a dedicated thread, a thread pool or a caller's thread.

Subscription to message stream can be done using rich fluent interface which is used to build a route from source Cell to concrete message consumer.

Event bus vs Actor programming style
------------------------------------

Nerve supports different approaches to consurrent message processing.

You can treat each Cells as an event bus instance, effectively decoupling producers from consumers:
```csharp
// creating a new cell
var bus = new Cell();
    
// subscribing to Ping signals
bus.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
    
// subscribing to Pong signals
bus.OnStream().Of<Pong>().ReactWith(_ => _received = true);
    
// firing (publishing) a signal
bus.Fire(new Ping());
    
// disposing the cell
bus.Dispose();
```

Or, you can use Cell as a base class for implementing different business entities interacting via message passing (like Actors):

```csharp
// defining ping actor object
public class PingActor : Cell
{
    public PingActor()
    {
        // subscribing to pong messages
        OnStream().Of<Pong>().ReactWith(s => Console.WriteLine("Received pong!"));
    }
}

// defining pong actor object
public class PongActor : Cell
{
    public PongActor(PingActor pingActor)
    {
        // subscribing to ping messages
        OnStream().Of<Ping>().ReactWith(s => pingActor.Fire(new Pong());
    }
}
    
var pinger = new PingActor();
var ponger = new PongActor(pinger);
    
// firing ping message
ponger.Fire(new Ping());
    
// disposing cells
pinger.Dispose();
ponger.Dispose();
```

Operators
---------

Nerve allows you to define a processing pipeline for messages passing through Cells. This allows for great flexibility.

```csharp
cell.OnStream()
    // filter by signal body type
    .Of<Ping>()
    // filter by content
    .Where(p => p.ShouldBeProcessed)
    // map to string
    .Map(p => p.ToString())
    // split incoming signal of string to multiple signals of char
    .Split(s => s.ToCharArray())
    // write each char to console
    .ReactWith(c => Console.WriteLine("Ping got : {0}", c));
```

Federation
----------

Each Cell can also be a message consumer itself. It can be subscribed to a stream of messages fired from another cells.

```csharp
// creating cells
var pinger = new Cell();
var ponger = new Cell();

// subscribing cells to signals from each other
pinger.OnStream().Of<Ping>().ReactWith(ponger);
ponger.OnStream().Of<Pong>().ReactWith(pinger);

// defining handlers on each cell
ponger.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
pinger.OnStream().Of<Pong>().ReactWith(_ => _received = true);

// firing (publishing) a signal
pinger.Fire(new Ping());
    
// disposing cells
pinger.Dispose();
ponger.Dispose();
```

Scheduling
----------

Each Cell processes messages (called Signals) using specific message scheduler. Scheduler decide how a signal will be processed. By default, incoming signals are processed on the same thread they were dispatched on. But there are other implementations, allowing to use dedicated thread or a thread pool.

```csharp
// a dedicated thread will be started with the Cell
var cell = new Cell(ThreadScheduler.Factory);

//...

// Ping will be handled on another thread
cell.Fire(new Ping());

// disposing cell and underlying thread
cell.Dispose();
```

Current State
-------------

This project is early in development. I'm open to ideas/pull requests.

License
-------
Licensed under Apache 2.0 License.

Using portions of [Retlang project](https://code.google.com/p/retlang/) licensed under [New BSD License](http://opensource.org/licenses/BSD-3-Clause).
