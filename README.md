Nerve
=====

Nerve is a federated messaging library for .NET. It combines ideas of [EventAggregator](http://martinfowler.com/eaaDev/EventAggregator.html), [Actors](http://en.wikipedia.org/wiki/Actor_model) and [Reactive Extensions](https://rx.codeplex.com/), allowing for fast and easy concurrent message handling.

Using
-----

The basic building block of Nerve infrastructure is an Agent, which, by default implementation, is basically an in-proc message broker. It allows you to subscribe to message stream using rich fluent interface and also to publish messages (called Signals).

A quick and simple example using single Agent:

    // creating a new agent
    _agent = new Agent();
    
    //subscribing to Ping signals
    _agent.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
    
    //subscribing to Pong signals
    _agent.OnStream().Of<Pong>().ReactWith(_ => _received = true);
    
    //dispatching (publishing) a signal
    _agent.Dispatch(new Ping());
    
    //disposing an agent
    _agent.Dispose();


Current State
-------------

This project is started as an experiment and I will continue to treat it this way for the time being. Meaning there'll be lots of breaking changes in the near future. And no NuGet distribution. But I'm open to ideas/pull requests, perhaps the project will grow into something useful.

License
-------
Licensed under Apache 2.0 License.
