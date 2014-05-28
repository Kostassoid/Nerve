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

namespace Kostassoid.Nerve.Core.Scheduling
{
	using System;
	using System.Collections.Concurrent;
	using System.Threading;

	/// <summary>
	/// Thread pool scheduler.
	/// </summary>
	public class ThreadScheduler : AbstractScheduler
	{
		readonly BlockingCollection<Action> _pending = new BlockingCollection<Action>();
		//readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
		Thread _thread;
		//readonly object _lock = new object();

		/// <summary>
		/// Scheduler factory.
		/// </summary>
		public static readonly Func<IScheduler> Factory = () => new ThreadScheduler();

		public ThreadScheduler()
		{
			_thread = new Thread(Run)
			{
				IsBackground = true
			};

			_thread.Start();
		}

		public override void Dispose()
		{
			if (_thread != null)
			{
				_pending.CompleteAdding();
				//_cancellation.Cancel();
				_thread.Join();
				_thread = null;
			}

			_pending.Dispose();
			//_cancellation.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Enqueue a single action.
		/// </summary>
		/// <param name="action"></param>
		public override void Enqueue(Action action)
		{
			_pending.Add(action);
		}

		void Run()
		{
			foreach (var act in _pending.GetConsumingEnumerable(/*_cancellation.Token*/))
			{
				act();
			}
		}
	}
}