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
	using System.Linq;

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

			private Because of = () => _cell.Send(new Ping());

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

			private Because of = () => _cell.Send(new Ping());

			private It should_not_be_handled = () => _received.ShouldBeFalse();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_intermediate_attached_cells
		{
			private static ICell _a;

			private static ICell _b;

			private static ICell _c;

			private static ISignal<Ping> _received;

			private Cleanup after = () =>
				{
					_a.Dispose();
					_b.Dispose();
					_c.Dispose();
				};

			private Establish context = () =>
				{
					_a = new Cell("a");
					_b = new Cell("b");
					_c = new Cell("c");

					_a.OnStream().Of<Ping>().ReactWith(_b);
					_b.Attach(_c);
					_c.OnStream().Of<Ping>().ReactWith(s => _received = s);
				};

			private Because of = () => _a.Send(new Ping());

			private It should_receive_signal = () => _received.ShouldNotBeNull();

			It should_have_all_handlers_on_stack =
				() => _received.Stacktrace.Frames
					.Select(f => f.ToString())
					.ShouldEqual(new[]
					{
						"Handler[SignalConsumerWrapper of Ping]",
						"Operator[Of of Ping]",
						"Cell[c]",
						"Cell[b]",
						"Operator[Of of Ping]",
						"Cell[a]"
					});
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

			private Because of = () => _cell.Send(new Pong());

			private It should_not_be_handled = () => _received.ShouldBeFalse();
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_back_using_concrete_handler
		{
			private static Cell _ping;

			private static Cell _pong;

			private static ISignal<Pong> _received;

			private Cleanup after = () =>
				{
					_ping.Dispose();
					_pong.Dispose();
				};

			private Establish context = () =>
				{
					_ping = new Cell("ping");
					_pong = new Cell("pong");

					_ping.OnStream().Of<Ping>().ReactWith(_pong);
					_pong.OnStream().Of<Pong>().ReactWith(_ping);

					_pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
					_ping.OnStream().Of<Pong>().ReactWith(s => _received = s);
				};

			private Because of = () => _ping.Send(new Ping());

			private It should_receive_response_on_specified_handler = () => _received.ShouldNotBeNull();

			It should_have_all_handlers_on_stack =
				() => _received.Stacktrace.Frames
					.Select(f => f.ToString())
					.ShouldEqual(new[]
					{
						"Handler[SignalConsumerWrapper of Pong]",
						"Operator[Of of Pong]",
						"Cell[ping]",
						"Handler[SignalConsumerWrapper of Ping]",
						"Operator[Of of Ping]",
						"Cell[pong]",
						"Operator[Of of Ping]",
						"Cell[ping]"
					});

		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}