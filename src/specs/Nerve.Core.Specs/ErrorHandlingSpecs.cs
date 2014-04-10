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

	using Machine.Specifications;

	using Model;

	using Processing.Operators;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ErrorHandlingSpecs
	{
		[Subject(typeof(Cell), "Error handling")]
		[Tags("Unit")]
		public class when_handler_throws_unhandled_exception_with_fault_handler_set
		{
			private static ICell _cell;

			private static bool _received;

			private static bool _cellIsNotified;

			private static bool _exceptionWasHandled;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_cell.Failed += (cell, exception) => { _cellIsNotified = true; };
					_cell.OnStream()
						.Of<Ping>()
						.ReactWith(_ => { throw new InvalidOperationException(); }, _ => { _exceptionWasHandled = true; return true; });
					_cell.OnStream().Of<Ping>().ReactWith(_ => { _received = true; });
				};

			private Because of = () => _cell.Send(new Ping());

			private It should_handle_exception = () => _exceptionWasHandled.ShouldBeTrue();

			private It should_not_disrupt_another_handlers = () => _received.ShouldBeTrue();

			private It should_not_notify_cell = () => _cellIsNotified.ShouldBeFalse();
		}

		[Subject(typeof(Cell), "Error handling")]
		[Tags("Unit")]
		public class when_handler_throws_unhandled_exception_without_fault_handler_set
		{
			private static ICell _cell;

			private static bool _received;

			private static bool _exceptionHasLeaked;

			private static bool _cellIsNotified;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_cell.Failed += (cell, exception) => _cellIsNotified = true;

					_cell.OnStream().Of<Ping>().ReactWith(_ => { throw new InvalidOperationException(); });
					_cell.OnStream().Of<Ping>().ReactWith(_ => { _received = true; });
				};

			private Because of = () =>
				{
					try
					{
						_cell.Send(new Ping());
					}
					catch
					{
						_exceptionHasLeaked = true;
					}
				};

			private It should_not_disrupt_another_handlers = () => _received.ShouldBeTrue();

			private It should_notify_cell = () => _cellIsNotified.ShouldBeTrue();

			private It should_swallow_exception = () => _exceptionHasLeaked.ShouldBeFalse();
		}

		[Subject(typeof(AbstractOperator), "Error handling")]
		[Tags("Unit")]
		public class when_operator_throws_exception
		{
			private static ICell _cell;

			private static bool _received;

			private static bool _cellIsNotified;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();
				_cell.Failed += (cell, exception) => { _cellIsNotified = true; };
				_cell.OnStream()
					.Of<Ping>()
					.Where(_ => { throw new InvalidOperationException(); })
					.ReactWith(_ => { _received = true; });
			};

			private Because of = () => _cell.Send(new Ping());

			private It should_not_continue_signal_processing = () => _received.ShouldBeFalse();

			private It should_notify_cell = () => _cellIsNotified.ShouldBeTrue();
		}

		[Subject(typeof(Signal), "Error handling")]
		[Tags("Unit")]
		public class when_handling_failure_with_callback_set
		{
			private static ICell _cell;
			private static ICell _callback;

			private static bool _received;
			private static bool _callbackIsNotified;

			private Cleanup after = () =>
				{
					_cell.Dispose();
					_callback.Dispose();
				};

			private Establish context = () =>
			{
				_cell = new Cell("original");
				_callback = new Cell("callback");
				_callback.Failed += (cell, exception) => { _callbackIsNotified = true; };
				_cell.OnStream()
					.Of<Ping>()
					.Where(_ => { throw new InvalidOperationException(); })
					.ReactWith(_ => { _received = true; });
			};

			private Because of = () => _cell.Send(new Ping(), _callback);

			private It should_not_continue_signal_processing = () => _received.ShouldBeFalse();

			private It should_be_handled_by_callback = () => _callbackIsNotified.ShouldBeTrue();
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}