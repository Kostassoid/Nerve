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
	using System.Collections.Generic;
	using System.Threading;

	/// <summary>
	/// Thread pool scheduler.
	/// </summary>
	public class ThreadScheduler : AbstractScheduler
	{
		Queue<Action> _pending = new Queue<Action>();
		Queue<Action> _temp = new Queue<Action>();
		Thread _thread;
		readonly object _lock = new object();
		bool _running = true;

		/// <summary>
		/// Scheduler factory.
		/// </summary>
		public static readonly Func<IScheduler> Factory = () => new ThreadScheduler();

		/// <summary>
		/// Initializes scheduler.
		/// </summary>
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
			lock (_lock)
			{
				_running = false;
				Monitor.PulseAll(_lock);
			}

			if (_thread != null)
			{
				_thread.Join();
				_thread = null;
			}

			base.Dispose();
		}

		/// <summary>
		/// Enqueue a single action.
		/// </summary>
		/// <param name="action"></param>
		public override void Enqueue(Action action)
		{
			lock (_lock)
			{
				_pending.Enqueue(action);
				if (_pending.Count == 1) Monitor.PulseAll(_lock);
			}
		}

		void Run()
		{
			lock (_lock)
			{
				while (_running)
				{
					Monitor.Wait(_lock);
					_temp = Interlocked.Exchange(ref _pending, _temp);

					while (_temp.Count > 0)
					{
						try
						{
							_temp.Dequeue()();
						}
						catch
						{ }
					}
				}
			}
		}
	}
}