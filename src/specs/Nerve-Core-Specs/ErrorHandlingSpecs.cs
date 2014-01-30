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

using System;

namespace Kostassoid.Nerve.Core.Specs
{
	using Machine.Specifications;
	using Model;
	using Pipeline;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ErrorHandlingSpecs
	{
		[Subject(typeof(ICell), "Error handling")]
		[Tags("Unit")]
		public class when_handler_throws_unhandled_exception_without_fault_handler_set
		{
			Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<SignalHandlingException>().ReactWith(_ => _cellIsNotified = true);

				_cell.OnStream().Of<Ping>().ReactWith(_ => { throw new InvalidOperationException(); });
				_cell.OnStream().Of<Ping>().ReactWith(_ => { _received = true; });
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () =>
			{
				try
				{
					_cell.Fire(new Ping());
				}
				catch
				{
					_exceptionHasLeaked = true;
				}
			};

			It should_not_disrupt_another_handlers = () => _received.ShouldBeTrue();

			It should_swallow_exception = () => _exceptionHasLeaked.ShouldBeFalse();

			It should_notify_cell = () => _cellIsNotified.ShouldBeTrue();

			static ICell _cell;
			static bool _received;
			static bool _exceptionHasLeaked;
			static bool _cellIsNotified;
		}

		[Subject(typeof(ICell), "Error handling")]
		[Tags("Unit")]
		public class when_handler_throws_unhandled_exception_with_fault_handler_set
		{
			Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<SignalHandlingException>().ReactWith(_ => _cellIsNotified = true);
				_cell.OnStream().Of<Ping>().ReactWith(_ => { throw new InvalidOperationException(); }, _ => { _exceptionWasHandled = true; });
				_cell.OnStream().Of<Ping>().ReactWith(_ => { _received = true; });
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_not_disrupt_another_handlers = () => _received.ShouldBeTrue();

			It should_handle_exception = () => _exceptionWasHandled.ShouldBeTrue();

			It should_not_notify_cell = () => _cellIsNotified.ShouldBeFalse();

			static ICell _cell;
			static bool _received;
			static bool _cellIsNotified;
			static bool _exceptionWasHandled;
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}