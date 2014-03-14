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
	using System;

	using Linking.Operators;

	using Machine.Specifications;

	using Model;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicRelaySpecs
	{
		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_attached_handler
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
				};

			private Because of = () => _cell.Fire(new Ping());

			private It should_be_handled = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_detached_handler
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					IDisposable subscription = _cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
					subscription.Dispose();
				};

			private Because of = () => _cell.Fire(new Ping());

			private It should_not_be_handled = () => _received.ShouldBeFalse();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_intermediate_attached_cells
		{
			private static ICell _a;

			private static ICell _b;

			private static ICell _c;

			private static bool _received;

			private Cleanup after = () =>
				{
					_a.Dispose();
					_b.Dispose();
					_c.Dispose();
				};

			private Establish context = () =>
				{
					_a = new Cell();
					_b = new Cell();
					_c = new Cell();

					_a.OnStream().Of<Ping>().ReactWith(_b);
					_b.OnStream().Of<Ping>().ReactWith(_c);
					_c.OnStream().Of<Ping>().ReactWith(_ => _received = true);
				};

			private Because of = () => _a.Fire(new Ping());

			private It should_receive_signal = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_without_attached_consumer
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<Ping>().ReactWith(_ => _received = true);
				};

			private Because of = () => _cell.Fire(new Pong());

			private It should_not_be_handled = () => _received.ShouldBeFalse();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_back_using_concrete_handler
		{
			private static Cell _ping;

			private static Cell _pong;

			private static bool _received;

			private Cleanup after = () =>
				{
					_ping.Dispose();
					_pong.Dispose();
				};

			private Establish context = () =>
				{
					_ping = new Cell();
					_pong = new Cell();

					_ping.OnStream().Of<Ping>().ReactWith(_pong);
					_pong.OnStream().Of<Pong>().ReactWith(_ping);

					_pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
					_ping.OnStream().Of<Pong>().ReactWith(_ => _received = true);
				};

			private Because of = () => _ping.Fire(new Ping());

			private It should_receive_response_on_specified_handler = () => _received.ShouldBeTrue();
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}