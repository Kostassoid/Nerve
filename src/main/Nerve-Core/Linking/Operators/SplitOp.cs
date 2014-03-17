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

	using Tools;

	public static class SplitOp
	{
		public static ILinkJunction<TOut> Split<TIn, TOut>(
			this ILinkJunction<TIn> step,
			Func<TIn, IEnumerable<TOut>> splitFunc) where TIn : class where TOut : class
		{
			var next = new SplitOperator<TIn, TOut>(step.Link, splitFunc);
			step.Attach(next);
			return next;
		}

		internal class SplitOperator<TIn, TOut> : AbstractOperator<TIn, TOut>
			where TIn : class where TOut : class
		{
			#region Fields

			private readonly Func<TIn, IEnumerable<TOut>> _splitFunc;

			#endregion

			#region Constructors and Destructors

			public SplitOperator(ILink link, Func<TIn, IEnumerable<TOut>> splitFunc)
				: base(link)
			{
				_splitFunc = splitFunc;
			}

			#endregion

			#region Public Methods and Operators

			public override void InternalProcess(ISignal<TIn> signal)
			{
				_splitFunc(signal.Payload).ForEach(b => Next.OnSignal(signal.CloneWithPayload(b)));
			}

			#endregion
		}
	}
}