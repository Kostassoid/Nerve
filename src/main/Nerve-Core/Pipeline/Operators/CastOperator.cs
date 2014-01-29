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

using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Pipeline.Operators
{
	internal class CastOperator<TOut> : AbstractOperator, ISynapseContinuation<TOut>
		where TOut : class
	{
		public CastOperator(Synapse synapse):base(synapse)
		{
		}

		public override void InternalProcess(ISignal signal)
		{
			var t = signal.Body as TOut;
			if (t == null) return;
			Next.Process(new Signal<TOut>(t, signal.StackTrace));
		}

		public void Attach(ISynapseOperator<TOut> next)
		{
			base.Attach(next);
		}
	}
}