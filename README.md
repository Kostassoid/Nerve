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

A basic building block of Nerve infrastructure is a Cell, which, by default implementation, is basically an in-proc queue with broadcast routing. All processing within a cell is serialized but can be scheduled on a dedicated thread, a thread pool or a caller's thread in case context switching is not acceptable.

Subscription to message stream can be done using rich fluent interface which is used to build a route from source Cell to concrete message consumer.

A quick and simple example using a single cell:

    // creating a new cell
    _cell = new Cell();
    
    //subscribing to Ping signals
    _cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
    
    //subscribing to Pong signals
    _cell.OnStream().Of<Pong>().ReactWith(_ => _received = true);
    
    //firing (publishing) a signal
    _cell.Fire(new Ping());
    
    //disposing the cell
    _cell.Dispose();

Federation
----------

Each Cell can also be a message consumer itself. It can be subscribed to a stream of messages fired from another cells, which allows for a really complex routing.

    // creating cells
    _ping = new Cell();
    _pong = new Cell();

    // subscribing cells to signals from each other
    _ping.OnStream().Of<Ping>().ReactWith(_pong);
    _pong.OnStream().Of<Pong>().ReactWith(_ping);

    // defining handlers on each cell
    _pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
    _ping.OnStream().Of<Pong>().ReactWith(_ => _received = true);

    //firing (publishing) a signal
    _ping.Fire(new Ping());
    
    //disposing cells
    _ping.Dispose();
    _pong.Dispose();

This example may not look any better than the first one. But imagine if one of the Cells is actually implemented as a client for some out-of-proc messaging middleware (ex. MSMQ).

Scheduling
----------

Each message (called Signals) passing through some route attached to a Cell travels through at least one scheduler. Scheduler decide how a signal will be delivered. By default, incoming signals are processed on the same thread they were dispatched on. But there are other implementations, allowing to use dedicated thread or a thread pool.

Current State
-------------

This project is early in development. I'm open to ideas/pull requests.

License
-------
Licensed under Apache 2.0 License.

Using portions of [Retlang project](https://code.google.com/p/retlang/) licensed under [New BSD License](http://opensource.org/licenses/BSD-3-Clause).