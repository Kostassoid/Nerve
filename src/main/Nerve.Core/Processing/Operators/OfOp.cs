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
	/// <summary>
	/// Of operator extension.
	/// </summary>
	public static class OfOp
	{
		/// <summary>
		/// Casts untyped signal to typed interface if possible. Filters signal otherwise.
		/// </summary>
		/// <typeparam name="TOut">Target payload type.</typeparam>
		/// <param name="step"></param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction<TOut> Of<TOut>(this ILinkJunction step)
		{
			var next = new OfOperator<TOut>(step.Link);
			step.Attach(next);
			return next;
		}

		internal class OfOperator<TOut> : AbstractOperator, ILinkJunction<TOut>
		{
			#region Constructors and Destructors

			public OfOperator(ILink link)
				: base(link)
			{
			}

			#endregion

			#region Public Methods and Operators

			protected override void Process(ISignal signal)
			{
				var typedSignal = signal as Signal<TOut>;
				if (typedSignal == null)
				{
					return;
				}
				Next.OnSignal(typedSignal);
			}

			#endregion
		}
	}
}