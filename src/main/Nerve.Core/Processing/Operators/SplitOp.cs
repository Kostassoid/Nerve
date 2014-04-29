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

namespace Kostassoid.Nerve.Core.Processing.Operators
{
	using System;
	using System.Collections.Generic;

	using Tools;

	/// <summary>
	/// Split operator extension.
	/// </summary>
	public static class SplitOp
	{
		/// <summary>
		/// Splits untyped signal into series of signals of another type.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="splitFunc">Split function.</param>
		/// <returns>Link extending point.</returns>
		public static ILinkJunction<TOut> Split<TOut>(
			this ILinkJunction step,
			Func<object, IEnumerable<TOut>> splitFunc)
		{
			var next = new SplitOperator<object, TOut>(step.Link, splitFunc);
			step.Attach(next);
			return next;
		}

		/// <summary>
		/// Splits typed signal into series of signals of another type.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="splitFunc">Split function.</param>
		/// <returns>Link extending point.</returns>
		public static ILinkJunction<TOut> Split<TIn, TOut>(
			this ILinkJunction<TIn> step,
			Func<TIn, IEnumerable<TOut>> splitFunc)
		{
			var next = new SplitOperator<TIn, TOut>(step.Link, splitFunc);
			step.Attach(next);
			return next;
		}

		internal class SplitOperator<TIn, TOut> : AbstractOperator<TIn, TOut>
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

			protected override void InternalProcess(ISignal<TIn> signal)
			{
				_splitFunc(signal.Payload).ForEach(b => Next.OnSignal(signal.WithPayload(b)));
			}

			#endregion
		}
	}
}