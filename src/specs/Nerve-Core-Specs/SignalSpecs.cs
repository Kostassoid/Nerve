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

	using Machine.Specifications;

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
					_headers = Headers.Empty.With("key", "value");
					_stack = Stacktrace.Empty;
					_originalSignal = new Signal<object>(new object(), _headers, _stack, _cell);
				};

			private Because of = () => _clonedSignal = _originalSignal.WithPayload("new payload");

			private It should_have_same_headers = () => _clonedSignal.Headers.ShouldBeTheSameAs(_headers);

			private It should_have_headers_set = () => _clonedSignal.Headers.ShouldEqual(_headers);

			private It should_have_payload_set = () => _clonedSignal.Payload.ShouldEqual("new payload");

			private It should_have_callback_receiver_set = () => _clonedSignal.Callback.ShouldEqual(_cell);

			private It should_have_same_stacktrace = () => _clonedSignal.Stacktrace.ShouldBeTheSameAs(_stack);

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
					stacktrace = Stacktrace.Empty.With(_cellA);
					stacktrace.With(_cellB);
				};

			private Because of = () => _signal = new Signal<object>(_payload, stacktrace);

			private It should_not_have_callback_receiver_set = () => _signal.Callback.ShouldBeNull();
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_creating_signal_with_new_stacktrace
		{
			static ICell _cell;

			static object _payload;

			static Headers _headers;

			static ISignal _signal;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
				{
					_cell = new Cell();
					_payload = new object();
					_headers = Headers.Empty.With("key", "value");
				};

			Because of = () => _signal = new Signal<object>(_payload, _headers, Stacktrace.Empty.With(_cell));

			It should_have_headers_set = () => _signal.Headers.ShouldEqual(_headers);

			It should_have_payload_set = () => _signal.Payload.ShouldEqual(_payload);

			It should_not_have_callback_receiver_set = () => _signal.Callback.ShouldBeNull();

			It should_have_stacktrace_set = () => _signal.Stacktrace.Frames.First().ShouldEqual(_cell);

			It should_not_have_exception_set = () => _signal.Exception.ShouldBeNull();
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
					_signal = new Signal<object>(new object(), Stacktrace.Empty.With(_cell));
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
					_signal = new Signal<object>(new object(), Stacktrace.Empty.With(_cell));
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

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_setting_signal_headers_to_non_null_value
		{
			private static ISignal _signal;

			private Establish context = () => { _signal = Signal.Of(new { }); };

			private Because of = () =>
			{
				_signal["a"] = "this";
				_signal["b"] = 13;
				_signal["a"] = "that";
			};

			private It should_have_headers_dictionary_with_added_items = () =>
			{
				_signal.Headers.Items.Count().ShouldEqual(2);

				_signal["a"].ShouldEqual("that");
				_signal["b"].ShouldEqual(13);
			};
		}

		[Subject(typeof(Signal<>))]
		[Tags("Unit")]
		public class when_setting_signal_headers_to_null_value
		{
			private static ISignal _signal;

			private Establish context = () =>
				{
					_signal = Signal.Of(new { });
					_signal["a"] = "this";
					_signal["b"] = 13;
					_signal["c"] = "that";
				};

			private Because of = () =>
			{
				_signal["b"] = null;
			};

			private It should_have_headers_dictionary_with_removed_items = () =>
			{
				_signal.Headers.Items.Count().ShouldEqual(2);

				_signal["a"].ShouldEqual("this");
				_signal["c"].ShouldEqual("that");
			};
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}