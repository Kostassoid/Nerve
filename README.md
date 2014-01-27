Nerve
=====

Nerve is a federated messaging library for .NET. It combines ideas of [EventAggregator](http://martinfowler.com/eaaDev/EventAggregator.html), [Actors](http://en.wikipedia.org/wiki/Actor_model) and [Reactive Extensions](https://rx.codeplex.com/), allowing for fast and easy concurrent message handling.

Basic usage
-----------

A basic building block of Nerve infrastructure is a Cell, which, by default implementation, is basically an in-proc message broker. It allows you to subscribe to message stream using rich fluent interface and also to publish messages (called Signals).

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

Each cell can also be a receiver itself. It can be subscribed to a stream of signals fired from another cells, which allows for building a really complex routing.

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

This example may not look impressive. But imagine if one of the cells is actually implemented as a client for some out-of-proc messaging middleware (ex. MSMQ).

Scheduling
----------

Each signal passing through attached pipeline (synapse) is going through at least one scheduler. Scheduler decide how a signal will be delivered. By default, incoming signals are processed on the same thread they were dispatched on. But there are other implementations, allowing to use dedicated thread or a thread pool.

Current State
-------------

This project is started as an experiment and I will continue to treat it this way for the time being. Meaning there'll be lots of breaking changes in the near future. And no NuGet distribution. But I'm open to ideas/pull requests, perhaps the project will grow into something useful.

License
-------
Licensed under Apache 2.0 License.
