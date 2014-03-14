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
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	using Linking.Operators;

	using Machine.Specifications;

	using Model;

	using Scheduling;

	using Tools;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
#pragma warning disable 0169
	public class PerformanceSpecs
	{
		[Behaviors]
		public class fast_message_broker
		{
			protected static int SignalsCount;

			protected static ICell Cell;

			private It should_be_faster_than_0_5_million_ops = () =>
				{
					var countdown = new CountdownEvent(SignalsCount);
					Cell.OnStream().Of<Ping>().ReactWith(_ => countdown.Signal());

					Stopwatch stopwatch = Stopwatch.StartNew();
					Enumerable.Range(0, SignalsCount).ForEach(_ => Cell.Fire(new Ping()));

					countdown.Wait();
					stopwatch.Stop();

					long ops = SignalsCount * 1000L / stopwatch.ElapsedMilliseconds;
					Console.WriteLine("Ops / second: {0}", ops);
					ops.ShouldBeGreaterThan(500000);
				};
		}

		[Subject(typeof(Cell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_immediate_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;

			private Behaves_like<fast_message_broker> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(ImmediateScheduler.Factory); };
		}

		[Subject(typeof(Cell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_pool_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;

			private Behaves_like<fast_message_broker> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(PoolScheduler.Factory); };
		}

		[Subject(typeof(Cell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_thread_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static ICell Cell;

			private Behaves_like<fast_message_broker> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(ThreadScheduler.Factory); };
		}
	}

#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}