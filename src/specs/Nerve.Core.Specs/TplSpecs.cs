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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Core.Tools;
	using Machine.Specifications;

	using Model;

	using Processing.Operators;
	using Scheduling;
	using Tpl;

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
	public class TplSpecs
	{
		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit")]
		public class when_requesting_object_using_task
		{
			static ICell _cell;

			static Task<string> _task;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return("pinged!"));
			};

			Because of = () => { _task = _cell.SendFor<string>(new Ping()); };

			It should_return_task = () => _task.ShouldNotBeNull();

			It should_have_value = () =>
			{
				_task.Wait(1000).ShouldBeTrue();
				_task.Result.ShouldEqual("pinged!");
			};
		}

		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit")]
		public class when_requesting_value_using_task
		{
			static ICell _cell;

			static Task<int> _task;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(13));
			};

			Because of = () => { _task = _cell.SendFor<int>(new Ping()); };

			It should_return_task = () => _task.ShouldNotBeNull();

			It should_have_value = () =>
			{
				_task.Wait(1000).ShouldBeTrue();
				_task.Result.ShouldEqual(13);
			};
		}

		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit")]
		public class when_using_invalid_response_type
		{
			static Cell _cell;

			static Task<string> _task;
			static Exception _exception;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
			};

			Because of = () => _exception = Catch.Exception(() =>
			{
				_task = _cell.SendFor<string>(new Ping());
				_task.Wait(1000);
			});

			It should_complete_the_task = () => _task.IsCompleted.ShouldBeTrue();

			It should_throw = () => _exception.InnerException.ShouldBeOfExactType<InvalidCastException>();

			It should_mark_task_as_failed = () => _task.IsFaulted.ShouldBeTrue();
		}

		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit", "Unstable")]
		public class when_requesting_many_values_using_tasks
		{
			const int SignalCount = 1000000;

			static Cell _cell;
			static CountdownEvent _countdown;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
			};

			It should_be_faster_than_0_1_million_ops = () =>
			{
				_countdown = new CountdownEvent(SignalCount);

				var stopwatch = Stopwatch.StartNew();

				Enumerable.Range(0, SignalCount)
					.ForEach(_ => _cell.SendFor<Pong>(new Ping()).ContinueWith(t =>
					{
						_countdown.Signal();
					}));

				_countdown.Wait();
				stopwatch.Stop();

				var ops = SignalCount * 1000L / stopwatch.ElapsedMilliseconds;
				Console.WriteLine("Ops / second: {0}", ops);
				ops.ShouldBeGreaterThan(100000);
			};
		}

		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit", "Unstable")]
		public class when_requesting_from_multiple_threads
		{
			const int SignalCount = 1000000;

			static Cell _cell;
			static CountdownEvent _countdown;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell(PoolScheduler.Factory);

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
			};

			It should_be_faster_than_0_1_million_ops = () =>
			{
				_countdown = new CountdownEvent(SignalCount);

				var stopwatch = Stopwatch.StartNew();

				Enumerable.Range(0, SignalCount)
					.ForEach(_ => Task.Factory.StartNew(() => _cell.SendFor<Pong>(new Ping()).ContinueWith(t =>
					{
						_countdown.Signal();
					})));

				_countdown.Wait();
				stopwatch.Stop();

				var ops = SignalCount * 1000L / stopwatch.ElapsedMilliseconds;
				Console.WriteLine("Ops / second: {0}", ops);
				ops.ShouldBeGreaterThan(100000);
			};
		}

	}

	// ReSharper restore UnusedMember.Local
	// ReSharper restore InconsistentNaming
}