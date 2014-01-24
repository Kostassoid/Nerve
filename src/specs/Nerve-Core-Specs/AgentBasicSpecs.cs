namespace Kostassoid.Nerve.Core.Specs
{
    using System;
    using System.Threading;
    using Machine.Specifications;

    // ReSharper disable InconsistentNaming
    public class AgentBasicSpecs
    {
        [Subject(typeof(IAgent), "Core")]
        [Tags("Unit")]
        public class when_dispatching_a_signal_with_registered_consumer
        {
            Establish context = () =>
            {
                _agent = new InProcAgent();

                _agent.OnStream().Of<Ping>().ReactWith(_ => _received = true);
            };

            Cleanup after = () => _agent.Dispose();

            Because of = () => _agent.Dispatch(new Ping());

            It should_be_handled = () => _received.ShouldBeTrue();

            static IAgent _agent;
            static bool _received;
        }

        [Subject(typeof(IAgent), "Core")]
        [Tags("Unit")]
        public class when_dispatching_a_signal_without_registered_consumer
        {
            Establish context = () =>
            {
                _agent = new InProcAgent();

                _agent.OnStream().Of<Ping>().ReactWith(_ => _received = true);
            };

            Cleanup after = () => _agent.Dispose();

            Because of = () => _agent.Dispatch(new Pong());

            It should_not_be_handled = () => _received.ShouldBeFalse();

            static IAgent _agent;
            static bool _received;
        }

        [Subject(typeof(IAgent), "Core")]
        [Tags("Unit")]
        public class when_dispatching_a_signal_with_intermediate_registered_agents
        {
            Establish context = () =>
            {
                _a = new InProcAgent();
                _b = new InProcAgent();
                _c = new InProcAgent();

                _a.OnStream().Of<Ping>().ReactWith(_b);
                _b.OnStream().Of<Ping>().ReactWith(_c);
                _c.OnStream().Of<Ping>().ReactWith(_ => _received = true);
            };

            Cleanup after = () =>
                            {
                                _a.Dispose();
                                _b.Dispose();
                                _c.Dispose();
                            };

            Because of = () => _a.Dispatch(new Ping());

            It should_receive_signal = () => _received.ShouldBeTrue();

            static IAgent _a;
            static IAgent _b;
            static IAgent _c;
            static bool _received;
        }

        [Subject(typeof(IAgent), "Core")]
        [Tags("Unit")]
        public class when_requesting_using_concrete_receiver
        {
            Establish context = () =>
                                {
                                    _ping = new InProcAgent();
                                    _pong = new InProcAgent();

                                    _ping.OnStream().Of<Ping>().ReactWith(ctx => ctx.Return(new Pong()));
                                    _pong.OnStream().Of<Pong>().ReactWith(_ => _received = true);
                                };

            Cleanup after = () =>
                            {
                                _ping.Dispose();
                                _pong.Dispose();
                            };

            Because of = () => _pong.Dispatch(new Ping());

            It should_receive_response_on_specified_receiver = () => _received.ShouldBeTrue();

            static InProcAgent _ping;
            static InProcAgent _pong;
            static bool _received;
        }

        [Subject(typeof(IAgent), "Core")]
        [Tags("Unit")]
        public class when_dispatching_using_async_scheduler
        {
            Establish context = () =>
            {
                _inProcAgent = new InProcAgent();
                _inProcAgent.OnStream().Of<Ping>()
                    //.ObserveOn(new EventLoopScheduler())
                    .ReactWith(_ =>
                    {
                        Thread.Sleep(100);
                        _waitHandle.Signal();
                    });
            };

            Cleanup after = () => _inProcAgent.Dispose();

            Because of = () =>
                         {
                             _inProcAgent.Dispatch(new Ping());
                             _inProcAgent.Dispatch(new Ping());
                             _inProcAgent.Dispatch(new Ping());
                         };

            It should_receive_in_async_fashion = () =>
                                                 {
                                                     _waitHandle.Wait(0).ShouldBeFalse();
                                                     _waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
                                                 };

            static InProcAgent _inProcAgent;
            static readonly CountdownEvent _waitHandle = new CountdownEvent(3);
        }
    }

    // ReSharper restore InconsistentNaming
}