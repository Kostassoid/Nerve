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

	public static class MapOp
	{
		public static ILinkJunction<TOut> Map<TIn, TOut>(this ILinkJunction<TIn> step, Func<TIn, TOut> mapFunc)
			where TIn : class where TOut : class
		{
			var next = new MapOperator<TIn, TOut>(step.Link, mapFunc);
			step.Attach(next);
			return next;
		}

		internal class MapOperator<TIn, TOut> : AbstractOperator<TIn, TOut>
			where TIn : class where TOut : class
		{
			#region Fields

			private readonly Func<TIn, TOut> _mapFunc;

			#endregion

			#region Constructors and Destructors

			public MapOperator(ILink link, Func<TIn, TOut> mapFunc)
				: base(link)
			{
				_mapFunc = mapFunc;
			}

			#endregion

			#region Public Methods and Operators

			public override void InternalProcess(ISignal<TIn> signal)
			{
				Next.OnSignal(signal.CloneWithPayload(_mapFunc(signal.Payload)));
			}

			#endregion
		}
	}
}