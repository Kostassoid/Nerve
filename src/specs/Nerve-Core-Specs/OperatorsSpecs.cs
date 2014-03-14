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

namespace Kostassoid.Nerve.Core.Specs
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using Linking;
	using Linking.Operators;

	using Machine.Specifications;

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

			private Because of = () => _cell.Fire(new SubSimpleNum { Num = 13 });

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

			private Because of = () => _cell.Fire(Enumerable.Range(1, 5).Select(i => new SimpleNum { Num = i }).ToList());

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

			private Because of = () => _cell.Fire(new SimpleNum { Num = 13 });

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

			private Because of = () => _cell.Fire(new SimpleNum { Num = 13 });

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

			private Because of = () => _cell.Fire(new SubSimpleNum { Num = 13 });

			private It should_not_be_received = () => _received.ShouldBeFalse();
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}