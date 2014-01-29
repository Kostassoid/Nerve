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

using Kostassoid.Nerve.Core.Scheduling;
using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Pipeline.Operators
{
	internal class ThroughOperator : AbstractOperator
	{
		private readonly IScheduler _scheduler;

		public ThroughOperator(Synapse synapse, IScheduler scheduler):base(synapse)
		{
			_scheduler = scheduler;
		}

		public override void InternalProcess(ISignal signal)
		{
			_scheduler.Fiber.Enqueue(() => Next.Process(signal));
		}
	}

	internal class ThroughOperator<T> : ThroughOperator, ISynapseContinuation<T> where T : class
	{
		public ThroughOperator(Synapse synapse, IScheduler scheduler)
			: base(synapse, scheduler)
		{
		}

		public void Attach(ISynapseOperator<T> next)
		{
			base.Attach(next);
		}
	}



}