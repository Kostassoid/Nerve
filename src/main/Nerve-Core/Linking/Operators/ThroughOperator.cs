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

	internal class ThroughOperator : AbstractOperator
	{
		private readonly IScheduler _scheduler;

		public ThroughOperator(ILink link, IScheduler scheduler):base(link)
		{
			_scheduler = scheduler;
		}

		public override void InternalProcess(ISignal signal)
		{
			_scheduler.Enqueue(() => Next.Process(signal));
		}
	}

	internal class ThroughOperator<T> : ThroughOperator/*, ILinkContinuation<T>*/ where T : class
	{
		public ThroughOperator(ILink link, IScheduler scheduler)
			: base(link, scheduler)
		{
		}

		public void Attach(ILinkOperator<T> next)
		{
			base.Attach(next);
		}
	}



}