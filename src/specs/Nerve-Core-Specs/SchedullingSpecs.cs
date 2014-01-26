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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Machine.Specifications;
	using Model;
	using Pipeline;
	using Scheduling;
	using Tools;

	// ReSharper disable UnusedField.Compiler
	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	#pragma warning disable 0169
	public class SchedullingSpecs
	{
		[Subject(typeof(IAgent), "Scheduling")]
		[Tags("Unit")]
		public class when_dispatching_using_async_scheduler
		{
			Establish context = () =>
			{
				_agent = new Agent();
				_agent.OnStream().Of<Ping>()
					.Through(new PoolScheduler())
					.ReactWith(_ =>
					{
						Thread.Sleep(100);
						_waitHandle.Signal();
					});
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () =>
			{
				_agent.Dispatch(new Ping());
				_agent.Dispatch(new Ping());
				_agent.Dispatch(new Ping());
			};

			It should_receive_in_async_fashion = () =>
			{
				_waitHandle.Wait(0).ShouldBeFalse();
				_waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
			};

			static Agent _agent;
			static readonly CountdownEvent _waitHandle = new CountdownEvent(3);
		}

		[Behaviors]
		public class serialized_processor
		{
			protected static IAgent Agent;
			protected static IScheduler Scheduler;

			It should_receive_in_original_order = () =>
			{
				var received = new List<int>();
				var countdown = new CountdownEvent(10);
				Agent.OnStream().Of<Num>().Through(Scheduler).ReactWith(s =>
																		{
																			received.Add(s.Body.Value);
																			countdown.Signal();
																		});

				Enumerable.Range(1, 10).ForEach(i => Agent.Dispatch(new Num(i)));

				countdown.Wait();

				received.ShouldEqual(received.OrderBy(r => r).ToList());
			};
		}

		[Subject(typeof(IAgent), "Scheduling")]
		[Tags("Unit")]
		public class when_dispatching_series_or_signals_using_immediate_scheduler
		{
			Establish context = () =>
			{
				Agent = new Agent();
				Scheduler = new ImmediateScheduler();
			};

			Cleanup after = () => Agent.Dispose();

			Behaves_like<serialized_processor> _;

			protected static IAgent Agent;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(IAgent), "Scheduling")]
		[Tags("Unit")]
		public class when_dispatching_series_or_signals_using_pool_scheduler
		{
			Establish context = () =>
			{
				Agent = new Agent();
				Scheduler = new PoolScheduler();
			};

			Cleanup after = () => Agent.Dispose();

			Behaves_like<serialized_processor> _;

			protected static IAgent Agent;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(IAgent), "Scheduling")]
		[Tags("Unit")]
		public class when_dispatching_series_or_signals_using_thread_scheduler
		{
			Establish context = () =>
			{
				Agent = new Agent();
				Scheduler = new ThreadScheduler();
			};

			Cleanup after = () => Agent.Dispose();

			Behaves_like<serialized_processor> _;

			protected static IAgent Agent;
			protected static IScheduler Scheduler;
		}


	}

	#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
	// ReSharper restore UnusedField.Compiler
}