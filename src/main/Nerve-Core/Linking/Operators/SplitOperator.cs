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
	using System;
	using System.Collections.Generic;
	using Signal;
	using Tools;

    internal class SplitOperator<TIn, TOut> : AbstractOperator<TIn, TOut>
		where TIn : class
		where TOut : class
	{
		private readonly Func<TIn, IEnumerable<TOut>> _splitFunc;

		public SplitOperator(ILink link, Func<TIn, IEnumerable<TOut>> splitFunc) : base(link)
		{
			_splitFunc = splitFunc;
		}

		public override void InternalProcess(ISignal<TIn> signal)
		{
		    _splitFunc(signal.Body)
                .ForEach(b => Next.Process(new Signal<TOut>(b, signal.StackTrace)));
		}
	}
}