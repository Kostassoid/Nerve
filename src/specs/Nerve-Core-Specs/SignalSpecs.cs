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

	using Kostassoid.Nerve.Core.Signal;

	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class SignalSpecs
	{
		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_creating_signal_with_new_stacktrace
		{
			Establish context = () =>
			{
				_cell = new Cell();
				_payload = new object();
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _signal = new Signal<object>(_payload, new StackTrace(_cell));

			It should_have_sender_set = () => _signal.Sender.ShouldEqual(_cell);

			It should_have_body_set = () => _signal.Sender.ShouldEqual(_cell);

			It should_have_stacktrace_set = () => _signal.StackTrace.Root.ShouldEqual(_cell);

			It should_not_have_exception_set = () => _signal.Exception.ShouldBeNull();

			static ICell _cell;
			static object _payload;
			static ISignal _signal;
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_creating_signal_with_existing_stacktrace
		{
			Establish context = () =>
			{
				_cellA = new Cell();
				_cellB = new Cell();
				_payload = new object();
				_stackTrace = new StackTrace(_cellA);
				_stackTrace.Push(_cellB);
			};

			Cleanup after = () =>
				{
					_cellA.Dispose();
					_cellB.Dispose();
				};

			Because of = () => _signal = new Signal<object>(_payload, _stackTrace);

			It should_have_sender_set_to_root_stack_cell = () => _signal.Sender.ShouldEqual(_cellA);

			static ICell _cellA;
			static ICell _cellB;
			static StackTrace _stackTrace;
			static object _payload;
			static ISignal _signal;
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_handling_exception_with_signal
		{
			Establish context = () =>
			{
				_cell = new Cell();
				_cell.Failed += (cell, exception) => { _cellIsNotified = true; };
				_signal = new Signal<object>(new object(), new StackTrace(_cell));
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _signal.HandleException(new Exception("uh"));

			It should_have_exception_set = () => _signal.Exception.ShouldNotBeNull();

			It should_have_exception_wrapped_in_signalexception = () => _signal.Exception.ShouldBeOfType<SignalException>();

			It should_notify_cell = () => _cellIsNotified.ShouldBeTrue();

			static ICell _cell;
			static ISignal _signal;
			static bool _cellIsNotified;
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_handling_exception_with_signal_already_marked_as_faulted
		{
			Establish context = () =>
			{
				_cell = new Cell();
				_signal = new Signal<object>(new object(), new StackTrace(_cell));
				_signal.HandleException(new Exception("uh"));
				_cell.Failed += (cell, exception) => { _cellIsNotified = true; };
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _exception = Catch.Exception(() => _signal.HandleException(new Exception("oh")));

			It should_throw_invalid_operation_exception = () => _exception.ShouldBeOfType<InvalidOperationException>();

			It should_not_change_existing_exception = () => _signal.Exception.InnerException.Message.ShouldEqual("uh");

			It should_not_notify_cell = () => _cellIsNotified.ShouldBeFalse();

			static ICell _cell;
			static ISignal _signal;
			static Exception _exception;
			static bool _cellIsNotified;
		}


	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}