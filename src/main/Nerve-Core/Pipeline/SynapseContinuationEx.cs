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

using System;
using Kostassoid.Nerve.Core.Pipeline.Operators;
using Kostassoid.Nerve.Core.Scheduling;
using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Pipeline
{
	public static class SynapseContinuationEx
	{
		public static ISynapseContinuation<TOut> Of<TOut>(this ISynapseContinuation step)
			where TOut : class
		{
			var next = new CastOperator<TOut>(step.Synapse);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation Where(this ISynapseContinuation step, Func<object, bool> predicateFunc)
		{
			var next = new FilterOperator(predicateFunc, step.Synapse);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation<T> Where<T>(this ISynapseContinuation<T> step, Func<T, bool> predicateFunc)
			where T : class
		{
			var next = new FilterOperator<T>(predicateFunc, step.Synapse);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation<T> Through<T>(this ISynapseContinuation<T> step, IScheduler scheduler)
			where T : class
		{
			var next = new ThroughOperator<T>(step.Synapse, scheduler);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation Through(this ISynapseContinuation step, IScheduler scheduler)
		{
			var next = new ThroughOperator(step.Synapse, scheduler);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation Gate(this ISynapseContinuation step, long minCount, ulong ms)
		{
			var next = new GateOperator(step.Synapse, minCount, ms);
			step.Attach(next);
			return next;
		}

		public static ISynapseContinuation<T> Gate<T>(this ISynapseContinuation<T> step, long minCount, ulong ms)
			where T : class
		{
			var next = new GateOperator<T>(step.Synapse, minCount, ms);
			step.Attach(next);
			return next;
		}

		public static IDisposable ReactWith<T>(this ISynapseContinuation<T> step, IHandler handler)
			where T : class
		{
			var next = new HandleOperator(handler);
			step.Attach(next);

			step.Synapse.Subscribe();
			return step.Synapse;
		}

		public static IDisposable ReactWith<T>(this ISynapseContinuation<T> step, IHandlerOf<T> handler)
			where T : class
		{
			var next = new HandleOperator<T>(handler);
			step.Attach(next);

			step.Synapse.Subscribe();
			return step.Synapse;
		}

		public static IDisposable ReactWith<T>(this ISynapseContinuation<T> step, Action<ISignal<T>> handler, Action<SignalHandlingException> failureHandler = null)
			where T : class
		{
			return ReactWith(step, new LambdaHandler<T>(handler, failureHandler));
		}

	}
}