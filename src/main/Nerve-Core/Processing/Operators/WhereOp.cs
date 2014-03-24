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

	/// <summary>
	/// Where operator extension.
	/// </summary>
	public static class WhereOp
	{
		/// <summary>
		/// Filters untyped signal stream using a predicate.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="predicateFunc">Predicate function.</param>
		/// <returns>Link extending point.</returns>
		public static ILinkJunction Where(this ILinkJunction step, Func<object, bool> predicateFunc)
		{
			var next = new FilterOperator(step.Link, predicateFunc);
			step.Attach(next);
			return next;
		}

		/// <summary>
		/// Filters typed signal stream using a predicate.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="predicateFunc">Predicate function.</param>
		/// <returns>Link extending point.</returns>
		public static ILinkJunction<T> Where<T>(this ILinkJunction<T> step, Func<T, bool> predicateFunc) where T : class
		{
			var next = new FilterOperator<T>(step.Link, predicateFunc);
			step.Attach(next);
			return next;
		}

		internal class FilterOperator<T> : AbstractOperator<T, T>
			where T : class
		{
			#region Fields

			private readonly Func<T, bool> _predicateFunc;

			#endregion

			#region Constructors and Destructors

			public FilterOperator(ILink link, Func<T, bool> predicateFunc)
				: base(link)
			{
				_predicateFunc = predicateFunc;
			}

			#endregion

			#region Public Methods and Operators

			protected override void InternalProcess(ISignal<T> signal)
			{
				if (!_predicateFunc(signal.Payload))
				{
					return;
				}
				Next.OnSignal(signal);
			}

			#endregion
		}

		internal class FilterOperator : AbstractOperator
		{
			#region Fields

			private readonly Func<object, bool> _predicateFunc;

			#endregion

			#region Constructors and Destructors

			public FilterOperator(ILink link, Func<object, bool> predicateFunc)
				: base(link)
			{
				_predicateFunc = predicateFunc;
			}

			#endregion

			#region Public Methods and Operators

			protected override void Process(ISignal signal)
			{
				if (!_predicateFunc(signal.Payload))
				{
					return;
				}
				Next.OnSignal(signal);
			}

			#endregion
		}
	}
}