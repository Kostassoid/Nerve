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
	using System.Threading;
	using Tools;
	using Tools.Collections;

	/// <summary>
	/// Thread pool scheduler.
	/// </summary>
	public class ThreadScheduler : AbstractScheduler
	{
		static int _threadId = 0;

		Thread _thread;
		AutoResetEvent _waitHandle = new AutoResetEvent(false);

		/// <summary>
		/// Initializes scheduler.
		/// </summary>
		public ThreadScheduler(IQueue<Action> queue) : base(queue)
		{
		}

		/// <summary>
		/// Initializes scheduler using UnboundedQueue.
		/// </summary>
		public ThreadScheduler() : this(new UnboundedQueue<Action>())
		{
		}

		public override void Dispose()
		{
			Stop();

			base.Dispose();
		}

		public override void Start()
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

			_waitHandle.Set ();
		}

		public override void Stop()
		{
			if (!IsRunning)
			{
				return;
			}

			base.Stop();

			_waitHandle.Set ();

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
			Pending.Enqueue(action);
			_waitHandle.Set ();
		}

		void Run()
		{
			while (IsRunning)
			{
				_waitHandle.WaitOne ();

				while (Pending.Count > 0)
				{
					Pending.DequeueAll ().ForEach (a => a ());
				}
			}
		}
	}
}