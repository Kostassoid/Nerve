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

namespace Kostassoid.Nerve.Core.Specs.Operators
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using Machine.Specifications;
	using Processing;
	using Processing.Operators;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class OperatorsSpecs
	{
		public class SimpleNum
		{
			public int Num;
		}

		public class SimpleString
		{
			public string Str;
		}

		public class SubSimpleNum : SimpleNum
		{
			public int SubNum;
		}

		[Subject(typeof(ILink), "Cast")]
		[Tags("Unit")]
		public class when_casting_untyped_signal_as_typed_with_non_exact_type_match
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Cast<SimpleNum>().ReactWith(s => _received = true);
				};

			private Because of = () => _cell.Send(new SubSimpleNum { Num = 13 });

			private It should_be_received = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(ILink), "Split")]
		[Tags("Unit")]
		public class when_splitting_signal_payload
		{
			private static ICell _cell;

			private static int _receivedSum;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<List<SimpleNum>>().Split(n => n.Select(i => i)).ReactWith(s => _receivedSum += s.Payload.Num);
				};

			private Because of = () => _cell.Send(Enumerable.Range(1, 5).Select(i => new SimpleNum { Num = i }).ToList());

			private It should_produce_multiple_signals = () => _receivedSum.ShouldEqual(1 + 2 + 3 + 4 + 5);
		}

		[Subject(typeof(ILink), "Map")]
		[Tags("Unit")]
		public class when_translating_signal_payload
		{
			private static ICell _cell;

			private static SimpleString _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream()
						.Of<SimpleNum>()
						.Map(n => new SimpleString { Str = n.Num.ToString(CultureInfo.InvariantCulture) })
						.ReactWith(s => _received = s.Payload);
				};

			private Because of = () => _cell.Send(new SimpleNum { Num = 13 });

			private It should_be_translated = () => _received.Str.ShouldEqual("13");
		}

		[Subject(typeof(ILink), "Of")]
		[Tags("Unit")]
		public class when_treating_untyped_signal_as_typed_with_exact_type_match
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<SimpleNum>().ReactWith(s => _received = true);
				};

			private Because of = () => _cell.Send(new SimpleNum { Num = 13 });

			private It should_be_received = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(ILink), "Of")]
		[Tags("Unit")]
		public class when_treating_untyped_signal_as_typed_with_non_exact_type_match
		{
			private static ICell _cell;

			private static bool _received;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell();

					_cell.OnStream().Of<SimpleNum>().ReactWith(s => _received = true);
				};

			private Because of = () => _cell.Send(new SubSimpleNum { Num = 13 });

			private It should_not_be_received = () => _received.ShouldBeFalse();
		}

		[Subject(typeof(ILink), "Catch")]
		[Tags("Unit")]
		public class when_exception_occured_with_exception_handler_is_set_on_link
		{
			private static ICell _cell;

			private static bool _received;
			private static bool _handledException;
			private static bool _cellIsNotified;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();

				_cell.Failed += (cell, exception) => { _cellIsNotified = true; };

				_cell.OnStream()
					.Of<SimpleString>()
					.Catch(_ => { _handledException = true; return true; })
					.Where(s => s.Str != null)
					.Map(s => new SimpleNum { Num = Int32.Parse(s.Str) })
					.ReactWith(s => _received = true);
			};

			private Because of = () => _cell.Send(new SimpleString { Str = "thirteen" });

			private It should_invoke_the_handler = () => _handledException.ShouldBeTrue();

			private It should_stop_processing = () => _received.ShouldBeFalse();

			private It should_not_notify_cell = () => _cellIsNotified.ShouldBeFalse();
		}


	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}