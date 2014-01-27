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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Machine.Specifications;
	using Model;
	using Pipeline;
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
			protected static IScheduler Scheduler;

			It should_be_faster_than_1_million_ops = () =>
			{
				var countdown = new CountdownEvent(SignalsCount);
				Cell.OnStream().Through(Scheduler).Of<Ping>().ReactWith(_ => countdown.Signal());

				var stopwatch = Stopwatch.StartNew();
				Enumerable.Range(0, SignalsCount).ForEach(_ => Cell.Fire(new Ping()));

				countdown.Wait();
				stopwatch.Stop();

				var ops = SignalsCount/stopwatch.ElapsedMilliseconds*1000;
				Console.WriteLine("Ops / second: {0}", ops);
				ops.ShouldBeGreaterThan(1000000);
			};
		}

		[Subject(typeof(ICell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_immediate_scheduler
		{
			Establish context = () =>
			{
				Cell = new Cell();
				Scheduler = new ImmediateScheduler();
			};

			Cleanup after = () =>
			{
				Scheduler.Dispose();
				Cell.Dispose();
			};

			Behaves_like<fast_message_broker> _;

			protected static int SignalsCount = 1000000;
			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(ICell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_pool_scheduler
		{
			Establish context = () =>
			{
				Cell = new Cell();
				Scheduler = new PoolScheduler();
			};

			Cleanup after = () =>
			{
				Scheduler.Dispose();
				Cell.Dispose();
			};

			Behaves_like<fast_message_broker> _;

			protected static int SignalsCount = 1000000;
			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(ICell), "Performance")]
		[Tags("Unit")]
		public class when_firing_many_signals_on_one_cell_using_thread_scheduler
		{
			Establish context = () =>
			{
				Cell = new Cell();
				Scheduler = new ThreadScheduler();
			};

			Cleanup after = () =>
			{
				Scheduler.Dispose();
				Cell.Dispose();
			};

			Behaves_like<fast_message_broker> _;

			protected static int SignalsCount = 1000000;
			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}
	}

	#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}