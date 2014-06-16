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
	public class PoolScheduler : AbstractScheduler
	{
		int _flushing;

		/// <summary>
		/// Initializes scheduler.
		/// </summary>
		public PoolScheduler(IQueue<Action> queue) : base(queue)
		{
		}

		/// <summary>
		/// Initializes scheduler using UnboundedQueue.
		/// </summary>
		public PoolScheduler() : this(new UnboundedQueue<Action>())
		{
		}

		/// <summary>
		/// Enqueue a single action.
		/// </summary>
		/// <param name="action"></param>
		public override void Enqueue(Action action)
		{
			Pending.Enqueue(action);

			if (!IsRunning || Interlocked.CompareExchange(ref _flushing, 1, 0) == 1)
			{
				return;
			}

			Flush();
		}

		/// <summary>
		/// Execute all actions in the pending list.  If any of the executed actions enqueue more actions, execute those as well.
		/// </summary>
		public void Flush()
		{
			ThreadPool.QueueUserWorkItem(_ =>
			{
				while (Pending.Count > 0)
				{
					Pending.DequeueAll().ForEach(a => a());
				}

				Interlocked.Exchange(ref _flushing, 0);
			});
		}
	}
}