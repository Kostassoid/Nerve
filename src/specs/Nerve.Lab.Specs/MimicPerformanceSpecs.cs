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

namespace Kostassoid.Nerve.Lab.Specs
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Core;
	using Core.Processing.Operators;
	using Machine.Specifications;
	using Mimic;
	using Core.Tools;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	#pragma warning disable 0169
	public class MimicPerformanceSpecs
	{
		public interface ILogic
		{
			void Foo(string p);
			string Boo(string p);
			Task<string> BooAsync(string p);
		}

		[Behaviors]
		public class fast_message_broker
		{
			protected static int SignalsCount;

			protected static ICell Cell;
			protected static Action InvokingAction;

			private It should_be_faster_than_0_5_million_ops = () =>
			{
				var countdown = new CountdownEvent(SignalsCount);

				Cell.OnStream().Of<Invocation>().ReactWith(s =>
					{
						s.Return("that");
						countdown.Signal();
					});

				var stopwatch = Stopwatch.StartNew();
				Enumerable.Range(0, SignalsCount).ForEach(_ => InvokingAction());

				countdown.Wait();
				stopwatch.Stop();

				var ops = SignalsCount * 1000L / stopwatch.ElapsedMilliseconds;
				Console.WriteLine("Ops / second: {0}", ops);
				ops.ShouldBeGreaterThan(500000);
			};
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit", "Unstable")]
		public class when_invoking_using_proxy_method_without_return_value
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;
			protected static Action InvokingAction;

			Cleanup after = () => Cell.Dispose();

			Establish context = () =>
			{
				Cell = new Cell();
				var logic = Cell.ProxyOf<ILogic>();
				InvokingAction = () => logic.Foo("this");
			};

			Behaves_like<fast_message_broker> _;
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit", "Unstable")]
		public class when_invoking_using_proxy_async_method
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;
			protected static Action InvokingAction;

			Cleanup after = () => Cell.Dispose();

			Establish context = () =>
			{
				Cell = new Cell();
				var logic = Cell.ProxyOf<ILogic>();
				InvokingAction = () => logic.BooAsync("this");
			};

			Behaves_like<fast_message_broker> _;
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit", "Unstable")]
		public class when_invoking_using_proxy_sync_method
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;
			protected static Action InvokingAction;

			Cleanup after = () => Cell.Dispose();

			Establish context = () =>
			{
				Cell = new Cell();
				var logic = Cell.ProxyOf<ILogic>();
				InvokingAction = () => logic.Boo("this");
			};

			Behaves_like<fast_message_broker> _;
		}

	}

	#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}