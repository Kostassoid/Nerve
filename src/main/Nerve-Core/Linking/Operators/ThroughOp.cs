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

namespace Kostassoid.Nerve.Core.Linking.Operators
{
	using Scheduling;

	using Signal;

	public static class ThroughOp
	{
		public static ILinkJunction<T> Through<T>(this ILinkJunction<T> step, IScheduler scheduler) where T : class
		{
			var next = new ThroughOperator<T>(step.Link, scheduler);
			step.Attach(next);
			return next;
		}

		public static ILinkJunction Through(this ILinkJunction step, IScheduler scheduler)
		{
			var next = new ThroughOperator(step.Link, scheduler);
			step.Attach(next);
			return next;
		}

		internal class ThroughOperator : AbstractOperator
		{
			#region Fields

			private readonly IScheduler _scheduler;

			#endregion

			#region Constructors and Destructors

			public ThroughOperator(ILink link, IScheduler scheduler)
				: base(link)
			{
				_scheduler = scheduler;
			}

			#endregion

			#region Public Methods and Operators

			public override void InternalProcess(ISignal signal)
			{
				_scheduler.Fiber.Enqueue(() => Next.OnSignal(signal));
			}

			#endregion
		}

		internal class ThroughOperator<T> : ThroughOperator, ILinkJunction<T>
			where T : class
		{
			#region Constructors and Destructors

			public ThroughOperator(ILink link, IScheduler scheduler)
				: base(link, scheduler)
			{
			}

			#endregion
		}
	}
}