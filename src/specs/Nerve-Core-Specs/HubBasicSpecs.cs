namespace Kostassoid.Nerve.Core.Specs
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using Machine.Specifications;

    // ReSharper disable InconsistentNaming
    public class HubBasicSpecs
    {
        [Subject(typeof(Hub), "Core")]
        [Tags("Unit")]
        public class when_requesting_using_concrete_receiver
        {
            Establish context = () =>
            {
                _hub = new Hub();

                _hub.On<Ping>().Subscribe(ctx => ctx.Reply(new Pong()));
            };

            Cleanup after = () => _hub.Dispose();

            Because of = () => _hub.Dispatch(new Ping(), Expect.When<Pong>(_ => _received = true));

            It should_receive_response_on_specified_receiver = () => _received.ShouldBeTrue();

            static Hub _hub;
            static bool _received;
        }

        [Subject(typeof(Hub), "Core")]
        [Tags("Unit")]
        public class when_dispatching_using_async_scheduler
        {
            Establish context = () =>
            {
                _hub = new Hub();
                _hub.On<Ping>()
                    .ObserveOn(new EventLoopScheduler())
                    .Subscribe(ctx =>
                    {
                        Thread.Sleep(100);
                        _waitHandle.Signal();
                    });
            };

            Cleanup after = () => _hub.Dispose();

            Because of = () =>
                         {
                             _hub.Dispatch(new Ping());
                             _hub.Dispatch(new Ping());
                             _hub.Dispatch(new Ping());
                         };

            It should_receive_in_async_fashion = () =>
                                                 {
                                                     _waitHandle.Wait(0).ShouldBeFalse();
                                                     _waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
                                                 };

            static Hub _hub;
            static readonly CountdownEvent _waitHandle = new CountdownEvent(3);
        }
    }

    // ReSharper restore InconsistentNaming
}