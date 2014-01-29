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

using System;
using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Pipeline.Operators
{
	internal class FilterOperator<T> : AbstractOperator<T, T>
		where T : class
	{
		private readonly Func<T, bool> _predicateFunc;

		public FilterOperator(Func<T, bool> predicateFunc, Synapse synapse) : base(synapse)
		{
			_predicateFunc = predicateFunc;
		}

		public override void InternalProcess(ISignal<T> signal)
		{
			if (!_predicateFunc(signal.Body)) return;
			Next.Process(signal);
		}
	}

	internal class FilterOperator : AbstractOperator
	{
		private readonly Func<object, bool> _predicateFunc;

		public FilterOperator(Func<object, bool> predicateFunc, Synapse synapse)
			: base(synapse)
		{
			_predicateFunc = predicateFunc;
		}

		public override void InternalProcess(ISignal signal)
		{
			if (!_predicateFunc(signal.Body)) return;
			Next.Process(signal);
		}
	}
}