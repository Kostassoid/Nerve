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
	using Tools.Collections;

	/// <summary>
	/// Base abstract signal processing scheduler.
	/// </summary>
	public abstract class AbstractScheduler : IScheduler
	{
		/// <summary>
		/// Pending queue.
		/// </summary>
		protected IQueue<Action> Pending { get; private set; }

		/// <summary>
		/// Initializes scheduler.
		/// </summary>
		protected AbstractScheduler(IQueue<Action> queue)
		{
			Pending = queue;
		}

		/// <summary>
		/// Initializes scheduler using UnboundedQueue.
		/// </summary>
		protected AbstractScheduler() : this(new UnboundedQueue<Action>())
		{
		}

		/// <summary>
		/// Disposes the scheduler, with all underlying threads and resources.
		/// </summary>
		public virtual void Dispose()
		{
		}

		public int QueueSize { get { return Pending.Count; } }

		public bool IsRunning { get; protected set; }

		/// <summary>
		/// Schedules new action.
		/// </summary>
		/// <param name="action">Action to schedule.</param>
		public abstract void Enqueue(Action action);

		public virtual void Start()
		{
			IsRunning = true;
		}

		public virtual void Stop()
		{
			IsRunning = false;
		}

	}
}