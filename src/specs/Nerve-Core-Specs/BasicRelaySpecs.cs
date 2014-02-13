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
	using Linking;
	using Machine.Specifications;
	using Model;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicRelaySpecs
	{
		[Subject(typeof(RelayCell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_attached_handler
		{
			Establish context = () =>
			{
				_cell = new RelayCell();

				_cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_be_handled = () => _received.ShouldBeTrue();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(RelayCell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_detached_handler
		{
			Establish context = () =>
			{
				_cell = new RelayCell();

				var subscription = _cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
				subscription.Dispose();
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_not_be_handled = () => _received.ShouldBeFalse();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(RelayCell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_without_attached_consumer
		{
			Establish context = () =>
								{
									_cell = new RelayCell();

									_cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
								};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Pong());

			It should_not_be_handled = () => _received.ShouldBeFalse();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(RelayCell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_intermediate_attached_cells
		{
			Establish context = () =>
								{
									_a = new RelayCell();
									_b = new RelayCell();
									_c = new RelayCell();

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

			Because of = () => _a.Fire(new Ping());

			It should_receive_signal = () => _received.ShouldBeTrue();

			static ICell _a;
			static ICell _b;
			static ICell _c;
			static bool _received;
		}

		[Subject(typeof(RelayCell), "Basic")]
		[Tags("Unit")]
		public class when_firing_back_using_concrete_handler
		{
			Establish context = () =>
								{
									_ping = new RelayCell();
									_pong = new RelayCell();

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

			Because of = () => _ping.Fire(new Ping());

			It should_receive_response_on_specified_handler = () => _received.ShouldBeTrue();

			static Cell _ping;
			static Cell _pong;
			static bool _received;
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}