namespace Kostassoid.Nerve.Core.Specs
{
	using Machine.Specifications;
	using Model;
	using Pipeline;

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
	public class RequestResponseSpecs
	{
		[Subject(typeof(IAgent), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_single_agent
		{
			Establish context = () =>
			{
				_agent = new Agent();

				_agent.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
				_agent.OnStream().Of<Pong>().ReactWith(_ => _received = true);
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () => _agent.Dispatch(new Ping());

			It should_receive_response = () => _received.ShouldBeTrue();

			static Agent _agent;
			static bool _received;
		}

		[Subject(typeof(IAgent), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_concrete_receiver
		{
			Establish context = () =>
			{
				_ping = new Agent();
				_pong = new Agent();

				_ping.OnStream().Of<Ping>().ReactWith(_pong);
				_pong.OnStream().Of<Pong>().ReactWith(_ping);

				_pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
				_ping.OnStream().Of<Pong>().ReactWith(_ => _received = true);
			};

			Cleanup after = () =>
			{
				_ping.Dispose();
				_pong.Dispose();
			};

			Because of = () => _ping.Dispatch(new Ping());

			It should_receive_response_on_specified_receiver = () => _received.ShouldBeTrue();

			static Agent _ping;
			static Agent _pong;
			static bool _received;
		}

		[Subject(typeof(IAgent), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_chain_of_agents
		{
			Establish context = () =>
			{
				_ping = new Agent("Ping");
				_middleman = new Agent("Middle man");
				_pong = new Agent("Pong");

				_ping.OnStream().Of<Ping>().ReactWith(_middleman);

				_middleman.OnStream().Of<Ping>().ReactWith(_pong);
				_middleman.OnStream().Of<Pong>().ReactWith(_ping);

				_pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
				_ping.OnStream().Of<Pong>().ReactWith(_ => _received = true);
			};

			Cleanup after = () =>
			{
				_ping.Dispose();
				_pong.Dispose();
				_middleman.Dispose();
			};

			Because of = () => _ping.Dispatch(new Ping());

			It should_receive_response_on_specified_receiver = () => _received.ShouldBeTrue();

			static Agent _ping;
			static Agent _middleman;
			static Agent _pong;
			static bool _received;
		}
	}

	// ReSharper restore UnusedMember.Local
	// ReSharper restore InconsistentNaming
}