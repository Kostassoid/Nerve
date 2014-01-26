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
	public class PerformanceSpecs
	{
		[Behaviors]
		public class fast_message_broker
		{
			protected static int SignalsCount;
			protected static IAgent Agent;
			protected static IScheduler Scheduler;

			It should_be_faster_than_1_million_ops = () =>
			{
				var countdown = new CountdownEvent(SignalsCount);
				Agent.OnStream().Of<Ping>().Through(Scheduler).ReactWith(_ => countdown.Signal());

				var stopwatch = Stopwatch.StartNew();
				Enumerable.Range(0, SignalsCount).ForEach(_ => Agent.Dispatch(new Ping()));

				countdown.Wait();
				stopwatch.Stop();

				(SignalsCount/stopwatch.ElapsedMilliseconds*1000).ShouldBeGreaterThan(1000000);
			};
		}

		[Subject(typeof(IAgent), "Performance")]
		[Tags("Unit")]
		public class when_publishing_many_signals_on_one_agent_using_immediate_scheduler
		{
			Establish context = () =>
			{
				Agent = new Agent();
				Scheduler = new ImmediateScheduler();
			};

			Cleanup after = () => Agent.Dispose();

			Behaves_like<fast_message_broker> _;

			protected static int SignalsCount = 1000000;
			protected static IAgent Agent;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(IAgent), "Performance")]
		[Tags("Unit")]
		public class when_publishing_many_signals_on_one_agent_using_pool_scheduler
		{
			Establish context = () =>
			{
				Agent = new Agent();
				Scheduler = new PoolScheduler();
			};

			Cleanup after = () => Agent.Dispose();

			Behaves_like<fast_message_broker> _;

			protected static int SignalsCount = 1000000;
			protected static IAgent Agent;
			protected static IScheduler Scheduler;
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}