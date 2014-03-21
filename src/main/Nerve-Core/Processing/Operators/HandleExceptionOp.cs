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

	public static class HandleExceptionOp
	{
		public static ILinkJunction HandleException(this ILinkJunction step, Func<SignalException, bool> failureHandlerFunc)
		{
			var next = new ExceptionHandlerOperator(step.Link, failureHandlerFunc);
			step.Attach(next);
			return next;
		}

		public static ILinkJunction<T> HandleException<T>(this ILinkJunction<T> step, Func<SignalException, bool> failureHandlerFunc) where T : class
		{
			var next = new ExceptionHandlerOperator<T>(step.Link, failureHandlerFunc);
			step.Attach(next);
			return next;
		}

		internal class ExceptionHandlerOperator<T> : AbstractOperator<T, T>
			where T : class
		{
			#region Fields

			private readonly Func<SignalException, bool> _failureHandlerFunc;

			#endregion

			#region Constructors and Destructors

			public ExceptionHandlerOperator(ILink link, Func<SignalException, bool> failureHandlerFunc)
				: base(link)
			{
				_failureHandlerFunc = failureHandlerFunc;
			}

			#endregion

			#region Public Methods and Operators

			public override void InternalProcess(ISignal<T> signal)
			{
				Next.OnSignal(signal);
			}

			public override bool OnFailure(SignalException exception)
			{
				return _failureHandlerFunc(exception);
			}

			#endregion
		}

		internal class ExceptionHandlerOperator : AbstractOperator
		{
			#region Fields

			private readonly Func<SignalException, bool> _failureHandlerFunc;

			#endregion

			#region Constructors and Destructors

			public ExceptionHandlerOperator(ILink link, Func<SignalException, bool> failureHandlerFunc)
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