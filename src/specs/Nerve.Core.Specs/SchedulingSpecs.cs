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
	using System.Threading.Tasks;

	using Machine.Specifications;

	using Model;

	using Processing.Operators;

	using Scheduling;

	using Core.Tools;

	// ReSharper disable UnusedField.Compiler
	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
#pragma warning disable 0169
	public class SchedulingSpecs
	{
		[Behaviors]
		public class serialized_processor
		{
			protected static ICell Cell;

			private It should_receive_in_original_order = () =>
				{
					var received = new List<int>();
					var countdown = new CountdownEvent(10);
					Cell.OnStream().Of<Num>().ReactWith(
						s =>
							{
								received.Add(s.Payload.Value);
								countdown.Signal();
							});

					Enumerable.Range(1, 10).ForEach(i => Cell.Send(new Num(i)));

					countdown.Wait();

					received.ShouldEqual(received.OrderBy(r => r).ToList());
				};
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_from_many_threads_using_serializing_scheduler
		{
			private const int _count = 20;

			private static readonly int[] _result = new int[_count];

			private static Cell _cell;

			private static readonly CountdownEvent _waitHandle = new CountdownEvent(_count);

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell(new ThreadScheduler());

					Action<ISignal<Num>> handler = s =>
						{
							Thread.Sleep(100);
							_result[s.Payload.Value]++;
							_waitHandle.Signal();
						};

					_cell.OnStream().Of<Num>().ReactWith(handler);
				};

			private Because of =
				() => Enumerable.Range(0, _count).ForEach(i => Task.Factory.StartNew(() => _cell.Send(new Num(i))));

			private It should_process_signals_serialized = () =>
				{
					_waitHandle.Wait(TimeSpan.FromSeconds(1)).ShouldBeFalse();
					_waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
					_result.All(i => i == 1).ShouldBeTrue();
				};
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_immediate_scheduler
		{
			protected static ICell Cell;

			private Behaves_like<serialized_processor> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(new ImmediateScheduler()); };
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_pool_scheduler
		{
			protected static ICell Cell;

			private Behaves_like<serialized_processor> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(new PoolScheduler()); };
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_series_or_signals_using_thread_scheduler
		{
			protected static ICell Cell;

			private Behaves_like<serialized_processor> _;

			private Cleanup after = () => Cell.Dispose();

			private Establish context = () => { Cell = new Cell(new ThreadScheduler()); };
		}

		[Subject(typeof(ICell), "Scheduling")]
		[Tags("Unit")]
		public class when_firing_using_async_scheduler
		{
			private static Cell _cell;

			private static readonly CountdownEvent _waitHandle = new CountdownEvent(3);

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
				{
					_cell = new Cell(new PoolScheduler());
					_cell.OnStream().Of<Ping>().ReactWith(
						_ =>
							{
								Thread.Sleep(100);
								_waitHandle.Signal();
							});
				};

			private Because of = () =>
				{
					_cell.Send(new Ping());
					_cell.Send(new Ping());
					_cell.Send(new Ping());
				};

			private It should_receive_in_async_fashion = () =>
				{
					_waitHandle.Wait(0).ShouldBeFalse();
					_waitHandle.Wait(TimeSpan.FromSeconds(3)).ShouldBeTrue();
				};
		}
	}

#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
	// ReSharper restore UnusedField.Compiler
}