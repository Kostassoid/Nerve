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
	/// Cast operator extension.
	/// </summary>
	public static class CastOp
	{
		/// <summary>
		/// Casts signal payload to another type. Changes signal type if succeedes.
		/// </summary>
		/// <typeparam name="TOut">Target type.</typeparam>
		/// <param name="step"></param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction<TOut> Cast<TOut>(this ILinkJunction step)
		{
			var next = new CastOperator<TOut>(step.Link);
			step.Attach(next);
			return next;
		}

		internal class CastOperator<TOut> : AbstractOperator, ILinkJunction<TOut>
		{
			#region Constructors and Destructors

			public CastOperator(ILink link)
				: base(link)
			{
			}

			#endregion

			#region Public Methods and Operators

			protected override void Process(ISignal signal)
			{
				if (!(signal.Payload is TOut))
				{
					return;
				}
				Next.OnSignal(signal.CastTo<TOut>());
			}

			#endregion
		}
	}
}