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
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using Linking;
	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class OperatorsSpecs
	{
		public class SimpleNum
		{
			public int Num;
		}

		public class SubSimpleNum : SimpleNum
		{
			public int SubNum;
		}

		public class SimpleString
		{
			public string Str;
		}

		[Subject(typeof(ILinkOperator), "Of")]
		[Tags("Unit")]
		public class when_treating_untyped_signal_as_typed_with_exact_type_match
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream()
					.Of<SimpleNum>()
					.ReactWith(s => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new SimpleNum { Num = 13 });

			It should_be_received = () => _received.ShouldBeTrue();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(ILinkOperator), "Of")]
		[Tags("Unit")]
		public class when_treating_untyped_signal_as_typed_with_non_exact_type_match
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream()
					.Of<SimpleNum>()
					.ReactWith(s => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new SubSimpleNum { Num = 13 });

			It should_not_be_received = () => _received.ShouldBeFalse();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(ILinkOperator), "Cast")]
		[Tags("Unit")]
		public class when_casting_untyped_signal_as_typed_with_non_exact_type_match
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream()
					.Cast<SimpleNum>()
					.ReactWith(s => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new SubSimpleNum { Num = 13 });

			It should_be_received = () => _received.ShouldBeTrue();

			static ICell _cell;
			static bool _received;
		}

		[Subject(typeof(ILinkOperator), "Map")]
		[Tags("Unit")]
		public class when_translating_signal_payload
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream()
					.Of<SimpleNum>()
					.Map(n => new SimpleString { Str = n.Num.ToString(CultureInfo.InvariantCulture) })
					.ReactWith(s => _received = s.Body);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new SimpleNum { Num = 13 });

			It should_be_translated = () => _received.Str.ShouldEqual("13");

			static ICell _cell;
			static SimpleString _received;
		}

		[Subject(typeof(ILinkOperator), "Split")]
		[Tags("Unit")]
		public class when_splitting_signal_payload
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream()
					.Of<List<SimpleNum>>()
					.Split(n => n.Select(i => i))
					.ReactWith(s => _receivedSum += s.Body.Num);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(Enumerable.Range(1, 5).Select(i => new SimpleNum { Num = i } ).ToList());

			It should_produce_multiple_signals = () => _receivedSum.ShouldEqual(1 + 2 + 3 + 4 + 5);

			static ICell _cell;
			static int _receivedSum;
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}