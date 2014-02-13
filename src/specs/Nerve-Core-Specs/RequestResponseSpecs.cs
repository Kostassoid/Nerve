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

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
	public class RequestResponseSpecs
	{
		[Subject(typeof(ICell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_single_cell
		{
			Establish context = () =>
			{
				_cell = new RelayCell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
				_cell.OnStream().Of<Pong>().ReactWith(_ => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_receive_response = () => _received.ShouldBeTrue();

			static Cell _cell;
			static bool _received;
		}

		[Subject(typeof(ICell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_concrete_handler
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

		[Subject(typeof(ICell), "Request")]
		[Tags("Unit")]
		public class when_requesting_using_chained_cells
		{
			Establish context = () =>
			{
				_ping = new RelayCell("Ping");
				_middleman = new RelayCell("Middle man");
				_pong = new RelayCell("Pong");

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

			Because of = () => _ping.Fire(new Ping());

			It should_receive_response_on_specified_handler = () => _received.ShouldBeTrue();

			static Cell _ping;
			static Cell _middleman;
			static Cell _pong;
			static bool _received;
		}
	}

	// ReSharper restore UnusedMember.Local
	// ReSharper restore InconsistentNaming
}