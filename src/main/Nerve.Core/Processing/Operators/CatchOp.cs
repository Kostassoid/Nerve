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
	/// Catch operator extension.
	/// </summary>
	public static class CatchOp
	{
		/// <summary>
		/// Defines failure handler on processing chain.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="failureHandlerFunc">Failure handler function.</param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction Catch(this ILinkJunction step, Func<SignalException, bool> failureHandlerFunc)
		{
			var next = new CatchOperator(step.Link, failureHandlerFunc);
			step.Attach(next);
			return next;
		}

		/// <summary>
		/// Defines failure handler on processing chain.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="failureHandlerFunc">Failure handler function.</param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction<T> Catch<T>(this ILinkJunction<T> step, Func<SignalException, bool> failureHandlerFunc)
		{
			var next = new CatchOperator<T>(step.Link, failureHandlerFunc);
			step.Attach(next);
			return next;
		}

		/// <summary>
		/// Defines failure handler on processing chain.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="failureHandlerSetup">Failure handler builder setup.</param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction Catch(this ILinkJunction step, Action<FailureHandlerBuilder> failureHandlerSetup)
		{
			var builder = new FailureHandlerBuilder();
			failureHandlerSetup(builder);

			var next = new CatchOperator(step.Link, builder.Build());
			step.Attach(next);
			return next;
		}

		/// <summary>
		/// Defines failure handler on processing chain.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="failureHandlerSetup">Failure handler builder setup.</param>
		/// <returns>Link extension point.</returns>
		public static ILinkJunction<T> Catch<T>(this ILinkJunction<T> step, Action<FailureHandlerBuilder<T>> failureHandlerSetup)
		{
			var builder = new FailureHandlerBuilder<T>();
			failureHandlerSetup(builder);

			var next = new CatchOperator<T>(step.Link, builder.Build());
			step.Attach(next);
			return next;
		}

		internal class CatchOperator<T> : AbstractOperator<T, T>
		{
			#region Fields

			private readonly Func<SignalException, bool> _failureHandlerFunc;

			#endregion

			#region Constructors and Destructors

			public CatchOperator(ILink link, Func<SignalException, bool> failureHandlerFunc)
				: base(link)
			{
				_failureHandlerFunc = failureHandlerFunc;
			}

			#endregion

			#region Public Methods and Operators

			protected override void InternalProcess(ISignal<T> signal)
			{
				Next.OnSignal(signal);
			}

			public override bool OnFailure(SignalException exception)
			{
				return _failureHandlerFunc(exception);
			}

			#endregion
		}

		internal class CatchOperator : AbstractOperator
		{
			#region Fields

			private readonly Func<SignalException, bool> _failureHandlerFunc;

			#endregion

			#region Constructors and Destructors

			public CatchOperator(ILink link, Func<SignalException, bool> failureHandlerFunc)
				: base(link)
			{
				_failureHandlerFunc = failureHandlerFunc;
			}

			#endregion

			#region Public Methods and Operators

			protected override void Process(ISignal signal)
			{
				Next.OnSignal(signal);
			}

			public override bool OnFailure(SignalException exception)
			{
				return _failureHandlerFunc(exception);
			}

			#endregion
		}
	}

}