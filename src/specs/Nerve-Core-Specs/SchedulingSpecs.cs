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

using System.Threading.Tasks;
using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Linking;
	using Machine.Specifications;
	using Model;
	using Scheduling;
	using Tools;

	// ReSharper disable UnusedField.Compiler
	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	#pragma warning disable 0169
	public class SchedulingSpecs
	{
		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_using_async_scheduler
		{
			Establish context = () =>
			{
				_cell = new RelayCell();
				_cell.OnStream().Of<Ping>()
					.Through(new PoolScheduler())
					.ReactWith(_ =>
					{
						Thread.Sleep(100);
						_waitHandle.Signal();
					});
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () =>
			{
				_cell.Fire(new Ping());
				_cell.Fire(new Ping());
				_cell.Fire(new Ping());
			};

			It should_receive_in_async_fashion = () =>
			{
				_waitHandle.Wait(0).ShouldBeFalse();
				_waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
			};

			static Cell _cell;
			static readonly CountdownEvent _waitHandle = new CountdownEvent(3);
		}

		[Behaviors]
		public class serialized_processor
		{
			protected static ICell Cell;
			protected static IScheduler Scheduler;

			It should_receive_in_original_order = () =>
			{
				var received = new List<int>();
				var countdown = new CountdownEvent(10);
				Cell.OnStream().Of<Num>().Through(Scheduler).ReactWith(s =>
				{
					received.Add(s.Body.Value);
					countdown.Signal();
				});

				Enumerable.Range(1, 10).ForEach(i => Cell.Fire(new Num(i)));

				countdown.Wait();

				received.ShouldEqual(received.OrderBy(r => r).ToList());
			};
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_immediate_scheduler
		{
			Establish context = () =>
			{
				Cell = new RelayCell();
				Scheduler = new ImmediateScheduler();
			};

			Cleanup after = () => Cell.Dispose();

			Behaves_like<serialized_processor> _;

			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_pool_scheduler
		{
			Establish context = () =>
			{
				Cell = new RelayCell();
				Scheduler = new PoolScheduler();
			};

			Cleanup after = () => Cell.Dispose();

			Behaves_like<serialized_processor> _;

			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_thread_scheduler
		{
			Establish context = () =>
			{
				Cell = new RelayCell();
				Scheduler = new ThreadScheduler();
			};

			Cleanup after = () => Cell.Dispose();

			Behaves_like<serialized_processor> _;

			protected static ICell Cell;
			protected static IScheduler Scheduler;
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_from_many_threads_using_serializing_scheduler
		{
			Establish context = () =>
			{
				_cell = new RelayCell();

				Action<ISignal<Num>> handler = s =>
				{
					Thread.Sleep(100);
					_result[s.Body.Value]++;
					_waitHandle.Signal();
				};

				_cell.OnStream()
					.Through(new ThreadScheduler())
					.Of<Num>()
					.ReactWith(handler);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => Enumerable.Range(0, _count).ForEach(i => Task.Factory.StartNew(() => _cell.Fire(new Num(i))));

			It should_process_signals_serialized = () =>
			{
				_waitHandle.Wait(TimeSpan.FromSeconds(1)).ShouldBeFalse();
				_waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
				_result.All(i => i == 1).ShouldBeTrue();
			};

			private const int _count = 20;
			static readonly int[] _result = new int[_count];
			static Cell _cell;
			static readonly CountdownEvent _waitHandle = new CountdownEvent(_count);
		}


	}

	#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
	// ReSharper restore UnusedField.Compiler
}