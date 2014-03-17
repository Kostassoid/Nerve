﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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
	using System;
	using System.Linq;

	using Machine.Specifications;

	using Cell = Core.Cell;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class SignalSpecs
	{
		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_cloning_signal_with_new_payload
		{
			private static ICell _cell;

			private static Headers _headers;

			private static Stacktrace _stack;

			private static ISignal _originalSignal;

			private static ISignal _clonedSignal;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_headers = new Headers { { "key", "value" } };
					_stack = new Stacktrace(_cell);
					_originalSignal = new Signal<object>(new object(), _headers, _stack);
				};

			private Because of = () => _clonedSignal = _originalSignal.CloneWithPayload("new payload");

			private It should_have_headers_copied = () => _clonedSignal.Headers.ShouldNotBeTheSameAs(_headers);

			private It should_have_headers_set = () => _clonedSignal.Headers.ShouldEqual(_headers);

			private It should_have_payload_set = () => _clonedSignal.Payload.ShouldEqual("new payload");

			private It should_have_sender_set = () => _clonedSignal.Sender.ShouldEqual(_cell);

			private It should_have_stacktrace_copied = () => _clonedSignal.Stacktrace.ShouldNotBeTheSameAs(_stack);

			private It should_have_stacktrace_set = () => _clonedSignal.Stacktrace.ShouldEqual(_stack);

			private It should_have_type_parameter_as_payload = () => _clonedSignal.ShouldBeOfExactType<Signal<string>>();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_creating_signal_with_existing_stacktrace
		{
			private static ICell _cellA;

			private static ICell _cellB;

			private static Stacktrace stacktrace;

			private static object _payload;

			private static ISignal _signal;

			private Cleanup after = () =>
				{
					_cellA.Dispose();
					_cellB.Dispose();
				};

			private Establish context = () =>
				{
					_cellA = new Cell();
					_cellB = new Cell();
					_payload = new object();
					stacktrace = new Stacktrace(_cellA);
					stacktrace.Trace(_cellB);
				};

			private Because of = () => _signal = new Signal<object>(_payload, stacktrace);

			private It should_have_sender_set_to_root_stack_cell = () => _signal.Sender.ShouldEqual(_cellA);
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_creating_signal_with_new_stacktrace
		{
			private static ICell _cell;

			private static object _payload;

			private static Headers _headers;

			private static ISignal _signal;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_payload = new object();
					_headers = new Headers { { "key", "value" } };
				};

			private Because of = () => _signal = new Signal<object>(_payload, _headers, new Stacktrace(_cell));

			private It should_have_headers_set = () => _signal.Headers.ShouldEqual(_headers);

			private It should_have_payload_set = () => _signal.Payload.ShouldEqual(_payload);

			private It should_have_sender_set = () => _signal.Sender.ShouldEqual(_cell);

			private It should_have_stacktrace_set = () => _signal.Stacktrace.Frames.First().ShouldEqual(_cell);

			private It should_not_have_exception_set = () => _signal.Exception.ShouldBeNull();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_marking_signal_as_faulted
		{
			private static ICell _cell;

			private static ISignal _signal;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_signal = new Signal<object>(new object(), new Stacktrace(_cell));
				};

			private Because of = () => _signal.MarkAsFaulted(new Exception("uh"));

			private It should_have_exception_set = () => _signal.Exception.ShouldNotBeNull();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_marking_faulted_signal_already_marked_as_faulted
		{
			private static ICell _cell;

			private static ISignal _signal;

			private static Exception _exception;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();
					_signal = new Signal<object>(new object(), new Stacktrace(_cell));
					_signal.MarkAsFaulted(new Exception("uh"));
				};

			private Because of = () => _exception = Catch.Exception(() => _signal.MarkAsFaulted(new Exception("oh")));

			private It should_not_change_existing_exception = () => _signal.Exception.Message.ShouldEqual("uh");

			private It should_throw_invalid_operation_exception = () => _exception.ShouldBeOfExactType<InvalidOperationException>();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_casting_signal_to_supported_type
		{
			class A { }
			class AA : A { }

			private static ISignal _signal;

			private static Exception _exception;

			private Establish context = () =>
			{
				_signal = new Signal<AA>(new AA(), Stacktrace.Empty);
			};

			private Because of = () => _exception = Catch.Exception(() => _signal.CastTo<A>());

			private It should_not_throw_exception = () => _exception.ShouldBeNull();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_casting_signal_to_unsupported_type
		{
			class A { }
			class B { }

			private static ISignal _signal;

			private static Exception _exception;

			private Establish context = () =>
			{
				_signal = new Signal<A>(new A(), Stacktrace.Empty);
			};

			private Because of = () => _exception = Catch.Exception(() => _signal.CastTo<B>());

			private It should_throw_invalid_cast_exception = () => _exception.ShouldBeOfExactType<InvalidCastException>();
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}