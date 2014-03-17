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

namespace Kostassoid.Nerve.Core.Specs.Cell
{
	using Linking.Operators;

	using Machine.Specifications;

	using Model;

	using Cell = Core.Cell;

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
	public class RequestResponseSpecs
	{
		[Subject(typeof(Cell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_chained_cells
		{
			private static Cell _ping;

			private static Cell _middleman;

			private static Cell _pong;

			private static bool _received;

			private Cleanup after = () =>
				{
					_ping.Dispose();
					_pong.Dispose();
					_middleman.Dispose();
				};

			private Establish context = () =>
				{
					_ping = new Cell("Ping");
					_middleman = new Cell("Middle man");
					_pong = new Cell("Pong");

					_ping.OnStream().Of<Ping>().ReactWith(_middleman);

					_middleman.OnStream().Of<Ping>().ReactWith(_pong);
					_middleman.OnStream().Of<Pong>().ReactWith(_ping);

					_pong.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
					_ping.OnStream().Of<Pong>().ReactWith(_ => _received = true);
				};

			private Because of = () => _ping.Fire(new Ping());

			private It should_receive_response_on_specified_handler = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(Cell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_concrete_handler
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

		[Subject(typeof(Cell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_single_cell
		{
			private static Cell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
					_cell.OnStream().Of<Pong>().ReactWith(_ => _received = true);
				};

			private Because of = () => _cell.Fire(new Ping());

			private It should_receive_response = () => _received.ShouldBeTrue();
		}
	}

	// ReSharper restore UnusedMember.Local
	// ReSharper restore InconsistentNaming
}