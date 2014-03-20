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

	using Tools;
	using Tools.CodeContracts;

	public static class ReactOp
	{
		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, ISignalProcessor signalProcessor) where T : class
		{
			step.Attach(signalProcessor);

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IConsumerOf<T> consumer) where T : class
		{
			step.Attach(new HandlerOperator<T>(consumer));

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(
			this ILinkJunction<T> step,
			Action<ISignal<T>> handler,
			Func<SignalException, bool> failureHandler = null) where T : class
		{
			step.Attach(new HandlerOperator<T>(handler, failureHandler));

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		internal class HandlerOperator<T> : SignalProcessor, IConsumerBase where T : class
		{
			protected IConsumerBase Original { get; set; }

			protected readonly Func<SignalException, bool> FailureHandler;

			protected readonly Action<ISignal<T>> Handler;

			public HandlerOperator(Action<ISignal<T>> handler, Func<SignalException, bool> failureHandler)
			{
				Requires.NotNull(handler, "signalProcessor");

				Handler = handler;
				FailureHandler = failureHandler;
				Original = this;
			}

			public HandlerOperator(IConsumerOf<T> consumer):this(consumer.OnSignal, consumer.OnFailure)
			{
				Original = consumer;
			}

			protected override void Process(ISignal signal)
			{
				Handler((ISignal<T>)signal);
			}

			public override bool OnFailure(SignalException exception)
			{
				if (FailureHandler == null) return false;

				return FailureHandler(exception);
			}

			public override string ToString()
			{
				return string.Format("Handler[{0}]", Original.GetType().BuildDescription());
			}

		}


	}
}