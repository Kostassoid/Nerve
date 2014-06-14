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
	using System.Linq;
	using System.Threading;
	using Tools;
	using Tools.Collections;

	/// <summary>
	/// Thread pool scheduler.
	/// </summary>
	public class ThreadScheduler2 : AbstractScheduler
	{
		static int _threadId = 1;

		IQueue<Action> _pending = new UnboundedQueue<Action>();
		Thread _thread;
		readonly object _lock = new object();

		/// <summary>
		/// Initializes scheduler.
		/// </summary>
		public ThreadScheduler2()
		{
		}

		public override void Dispose()
		{
			Stop();

			base.Dispose();
		}

		public override int QueueSize
		{
			get
			{
				return _pending.Count;
			}
		}

		public override void Start()
		{
			lock (_lock)
			{
				if (IsRunning) return;
				base.Start();

				var id = Interlocked.Increment(ref _threadId);
				_thread = new Thread(Run)
				{
					IsBackground = true,
					Name = string.Format("ThreadScheduler-{0}", id),
					Priority = ThreadPriority.Normal
				};

				_thread.Start();

				Monitor.PulseAll(_lock);
			}
		}

		public override void Stop()
		{
			lock (_lock)
			{
				if (!IsRunning)
				{
					return;
				}

				base.Stop();

				Monitor.PulseAll(_lock);
			}

			if (_thread != null)
			{
				_thread.Join();
				_thread = null;
			}
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
				while (IsRunning)
				{
					while (_pending.Count > 0)
					{
						_pending.DequeueAll().ForEach(a => a());
					}
					Monitor.Wait(_lock);
				}
			}
		}
	}
}