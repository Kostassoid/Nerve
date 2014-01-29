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
	public abstract class AbstractOperator : ISynapseOperator, ISynapseContinuation
	{
		private ISynapseOperator _next;

		public ISynapse Synapse { get; private set; }
		public ISynapseOperator Next { get { return _next; }}

		protected AbstractOperator(ISynapse synapse)
		{
			Synapse = synapse;
		}

		public void Process(ISignal signal)
		{
			if (_next == null) return;

			InternalProcess(signal);
		}

		public abstract void InternalProcess(ISignal signal);

		public void Attach(ISynapseOperator next)
		{
			_next = next;
		}

		public override string ToString()
		{
			//TODO: strip Operator from name, convert '1 to generic param name
			return string.Format("Operator [{0}]", GetType().Name);
		}
	}

	public abstract class AbstractOperator<TIn, TOut> : AbstractOperator, ISynapseOperator<TIn>, ISynapseContinuation<TOut>
		where TIn : class
		where TOut : class
	{
		protected AbstractOperator(ISynapse synapse):base(synapse)
		{
		}

		public override void InternalProcess(ISignal signal)
		{
			var s = signal as ISignal<TIn>;
			if (s == null) return;

			InternalProcess(s);
		}

		public abstract void InternalProcess(ISignal<TIn> signal);

		public void Process(ISignal<TIn> signal)
		{
			if (Next == null) return;

			InternalProcess(signal);
		}

		public void Attach(ISynapseOperator<TOut> next)
		{
			base.Attach(next);
		}
	}
}