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

namespace Kostassoid.Nerve.Core.Specs.Tools
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Core.Tools.Collections;
	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class QueueSpecs
	{
		[Subject(typeof(IQueue<>))]
		[Tags("Unit")]
		public class when_creating_unbounded_queue
		{
			static IQueue<int> _queue;

			Because of = () =>
			{
				_queue = new UnboundedQueue<int>();
			};

			It should_have_zero_size = () => _queue.Count.ShouldEqual(0);

			It should_be_empty = () => _queue.DequeueAll().ShouldBeEmpty();
		}

		[Subject(typeof(IQueue<>))]
		[Tags("Unit")]
		public class when_enqueing_to_unbounded_queue
		{
			static IQueue<int> _queue;

			Because of = () =>
			{
				_queue = new UnboundedQueue<int>();
				_queue.Enqueue(1);
				_queue.Enqueue(3);
				_queue.Enqueue(2);
			};

			It should_have_correct_size = () => _queue.Count.ShouldEqual(3);

			It should_dequeue_in_correct_order = () =>
			{
				var list = _queue.DequeueAll();
				list.ShouldBeLike(new[] { 1, 3, 2 });
			};
		}

		[Subject(typeof(IQueue<>))]
		[Tags("Unit")]
		public class when__from_another_thread
		{
			const int Items = 10000;
			static Task[] _tasks;
			static IQueue<int> _queue;
			static int _sum;

			Because of = () =>
			{
				_queue = new UnboundedQueue<int>();

				_tasks = Enumerable
					.Range(0, Items)
					.Select(i => Task.Factory.StartNew(() => _queue.Enqueue(i)))
					.ToArray();
			};

			It should_dequeue = () =>
			{
				Task.WaitAll(_tasks);

				var sumTask = Task.Factory.StartNew(() =>
				{
					foreach (var item in _queue.DequeueAll())
					{
						_sum += item;
					}
				});

				sumTask.Wait();

				_sum.ShouldEqual(Enumerable.Range(0, Items).Sum());
			};
		}


	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}