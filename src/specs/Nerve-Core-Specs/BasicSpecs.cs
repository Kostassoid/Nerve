// Copyright 2014 https://github.com/Kostassoid/Nerve
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Nerve.Core.Specs
{
	using Machine.Specifications;
	using Model;
	using Pipeline;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicSpecs
	{
		[Subject(typeof(IAgent), "Basic")]
		[Tags("Unit")]
		public class when_dispatching_a_signal_with_registered_consumer
		{
			Establish context = () =>
			{
				_agent = new Agent();

				_agent.OnStream().Of<Ping>().ReactWith(_ => _received = true);
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () => _agent.Dispatch(new Ping());

			It should_be_handled = () => _received.ShouldBeTrue();

			static IAgent _agent;
			static bool _received;
		}

		[Subject(typeof(IAgent), "Basic")]
		[Tags("Unit")]
		public class when_dispatching_a_signal_with_unregistered_consumer
		{
			Establish context = () =>
			{
				_agent = new Agent();

				var subscription = _agent.OnStream().Of<Ping>().ReactWith(_ => _received = true);
				subscription.Dispose();
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () => _agent.Dispatch(new Ping());

			It should_not_be_handled = () => _received.ShouldBeFalse();

			static IAgent _agent;
			static bool _received;
		}

		[Subject(typeof(IAgent), "Basic")]
		[Tags("Unit")]
		public class when_dispatching_a_signal_without_registered_consumer
		{
			Establish context = () =>
								{
									_agent = new Agent();

									_agent.OnStream().Of<Ping>().ReactWith(_ => _received = true);
								};

			Cleanup after = () => _agent.Dispose();

			Because of = () => _agent.Dispatch(new Pong());

			It should_not_be_handled = () => _received.ShouldBeFalse();

			static IAgent _agent;
			static bool _received;
		}

		[Subject(typeof(IAgent), "Basic")]
		[Tags("Unit")]
		public class when_dispatching_a_signal_with_intermediate_registered_agents
		{
			Establish context = () =>
								{
									_a = new Agent();
									_b = new Agent();
									_c = new Agent();

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

		[Subject(typeof(IAgent), "Basic")]
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

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}