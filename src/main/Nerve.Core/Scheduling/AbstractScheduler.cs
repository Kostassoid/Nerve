﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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

	using Fibers;

	/// <summary>
	/// Base abstract signal processing scheduler.
	/// </summary>
	public abstract class AbstractScheduler : IScheduler
	{
		private IFiber _fiber;

		internal IFiber Fiber
		{
			get
			{
				if (_fiber != null)
				{
					return _fiber;
				}

				_fiber = BuildFiber();
				_fiber.Start();

				return _fiber;
			}
		}

		/// <summary>
		/// Disposes the scheduler, with all underlying threads and resources.
		/// </summary>
		public void Dispose()
		{
			if (_fiber != null)
			{
				_fiber.Dispose();
			}
		}

		/// <summary>
		/// Schedules new action.
		/// </summary>
		/// <param name="action">Action to schedule.</param>
		public void Schedule(Action action)
		{
			Fiber.Enqueue(action);
		}

		internal abstract IFiber BuildFiber();
	}
}