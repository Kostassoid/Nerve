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

namespace Kostassoid.Nerve.Core.Linking
{
	using System;
	using System.Collections.Generic;

	using Operators;
	using Scheduling;
	using Signal;

	public static class LinkContinuationEx
	{
		public static ILinkContinuation<TOut> Of<TOut>(this ILinkContinuation step)
			where TOut : class
		{
			var next = new OfOperator<TOut>(step.Link);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<TOut> Cast<TOut>(this ILinkContinuation step)
			where TOut : class
		{
			var next = new CastOperator<TOut>(step.Link);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation Where(this ILinkContinuation step, Func<object, bool> predicateFunc)
		{
			var next = new FilterOperator(step.Link, predicateFunc);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<T> Where<T>(this ILinkContinuation<T> step, Func<T, bool> predicateFunc)
			where T : class
		{
			var next = new FilterOperator<T>(step.Link, predicateFunc);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<T> Through<T>(this ILinkContinuation<T> step, IScheduler scheduler)
			where T : class
		{
			var next = new ThroughOperator<T>(step.Link, scheduler);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation Through(this ILinkContinuation step, IScheduler scheduler)
		{
			var next = new ThroughOperator(step.Link, scheduler);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation Gate(this ILinkContinuation step, int threshold, TimeSpan timespan)
		{
			var next = new GateOperator(step.Link, threshold, timespan);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<T> Gate<T>(this ILinkContinuation<T> step, int threshold, TimeSpan timespan)
			where T : class
		{
			var next = new GateOperator<T>(step.Link, threshold, timespan);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<TOut> Map<TIn, TOut>(this ILinkContinuation<TIn> step, Func<TIn, TOut> mapFunc)
			where TIn : class
			where TOut : class
		{
			var next = new MapOperator<TIn, TOut>(step.Link, mapFunc);
			step.Attach(next);
			return next;
		}

		public static ILinkContinuation<TOut> Split<TIn, TOut>(this ILinkContinuation<TIn> step, Func<TIn, IEnumerable<TOut>> splitFunc)
			where TIn : class
			where TOut : class
		{
			var next = new SplitOperator<TIn, TOut>(step.Link, splitFunc);
			step.Attach(next);
			return next;
		}

		public static IDisposable ReactWith<T>(this ILinkContinuation<T> step, IConsumer consumer)
			where T : class
		{
			var next = new HandleOperator(consumer);
			step.Attach(next);

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(this ILinkContinuation<T> step, IConsumerOf<T> consumer)
			where T : class
		{
			var next = new HandleOperator<T>(consumer);
			step.Attach(next);

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(this ILinkContinuation<T> step, Action<ISignal<T>> handler, Action<SignalException> failureHandler = null)
			where T : class
		{
			return ReactWith(step, new LambdaConsumer<T>(handler, failureHandler));
		}

	}
}