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
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Machine.Specifications;
	using Scheduling;
	using Core.Tools;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
#pragma warning disable 0169
	public class ConcurrencySpecs
	{
		[Behaviors]
        public class valid_scheduler
		{
			protected static int SignalsCount;

			protected static IScheduler Scheduler;

            private It should_process_all_signals = () =>
				{
					var countdown = new CountdownEvent(SignalsCount);

					var tasks = Enumerable
						.Range(0, SignalsCount)
						.Select(_ => Task.Factory.StartNew(() => Scheduler.Enqueue(() => countdown.Signal())))
						.ToArray();

					Task.WaitAll(tasks, 10000).ShouldBeTrue();

					countdown.Wait(10000).ShouldBeTrue();
				};
		}

		[Subject(typeof(IScheduler), "Concurrency")]
		[Tags("Unit", "Unstable")]
		public class when_scheduling_using_immediate_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static IScheduler Scheduler;

			Behaves_like<valid_scheduler> _;

			Cleanup after = () => Scheduler.Dispose();

			Establish context = () => { Scheduler = ImmediateScheduler.Factory(); };
		}

		[Subject(typeof(IScheduler), "Concurrency")]
		[Tags("Unit", "Unstable")]
		public class when_firing_many_signals_on_one_cell_using_pool_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static IScheduler Scheduler;

			Behaves_like<valid_scheduler> _;

			Cleanup after = () => Scheduler.Dispose();

			Establish context = () => { Scheduler = PoolScheduler.Factory(); };
		}

		[Subject(typeof(IScheduler), "Concurrency")]
		[Tags("Unit", "Unstable")]
		public class when_firing_many_signals_on_one_cell_using_thread_scheduler
		{
			protected static int SignalsCount = 1000000;

			protected static IScheduler Scheduler;

			Behaves_like<valid_scheduler> _;

			Cleanup after = () => Scheduler.Dispose();

			Establish context = () => { Scheduler = ThreadScheduler.Factory(); };
		}
	}

#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}