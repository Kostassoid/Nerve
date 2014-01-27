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

namespace Kostassoid.Nerve.Core.Pipeline
{
	using System;
	using Scheduling;
	using Signal;

	public static class PipelineEx
	{
		public static IPipelineStep<TOut> Map<TIn, TOut>(this IPipelineStep<TIn> step, Func<TIn, TOut> mapFunc)
			where TOut : class
			where TIn : class
		{
			var next = new PipelineStep<TOut>(step.Link);
			step.Attach(s => next.Execute(new Signal<TOut>(mapFunc(s.Body), s.StackTrace)));

			return next;
		}

		public static IPipelineStep<TOut> Of<TOut>(this IPipelineStep step)
			where TOut : class
		{
			var next = new PipelineStep<TOut>(step.Link);
			step.Attach(s =>
							   {
								   var t = s.Body as TOut;
								   if (t == null) return;
								   next.Execute(new Signal<TOut>(t, s.StackTrace));
							   });

			return next;
		}

		public static IPipelineStep<T> Where<T>(this IPipelineStep<T> step, Func<T, bool> predicate)
			where T : class
		{
			var next = new PipelineStep<T>(step.Link);
			step.Attach(s =>
						{
							if (predicate(s.Body))
								next.Execute(s);
						});

			return next;
		}

		public static IPipelineStep Through(this IPipelineStep step, IScheduler scheduler)
		{
			var next = new PipelineStep<object>(step.Link) as IPipelineStep;
			step.Attach(s => scheduler.Fiber.Enqueue(() => next.Execute(s)));
			return next;
		}

		public static IPipelineStep<T> Through<T>(this IPipelineStep<T> step, IScheduler scheduler)
			where T : class
		{
			var next = new PipelineStep<T>(step.Link);
			step.Attach(s => scheduler.Fiber.Enqueue(() => next.Execute(s)));
			return next;
		}

		public static IDisposable ReactWith<T>(this IPipelineStep<T> step, IConsumerOf<T> handler)
			where T : class
		{
			step.Attach(handler.Handle);
			step.Link.Subscribe();
			return step.Link;
		}

		public static IDisposable ReactWith<T>(this IPipelineStep<T> step, Action<ISignal<T>> handler)
			where T : class
		{
			step.Attach(handler);
			step.Link.Subscribe();
			return step.Link;
		}

		public static IDisposable ReactWith(this IPipelineStep step, IAgent handler)
		{
			step.Attach(handler.Handle);
			step.Link.Subscribe();
			return step.Link;
		}
	}
}