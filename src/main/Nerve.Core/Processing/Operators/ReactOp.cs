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
	/// React operator extension.
	/// </summary>
	public static class ReactOp
	{
		/// <summary>
		/// Defines a processor to handle typed signal stream.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="step"></param>
		/// <param name="processor">Handling processor.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IProcessor processor)
		{
			step.Attach(processor);

			//TODO: not pretty
			return step.Link.AttachToSource();
		}

		/// <summary>
		/// Defines a consumer for untyped signal stream.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="consumer">Consumer.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public static IDisposable ReactWith(this ILinkJunction step, IConsumer consumer)
		{
			step.Attach(new ConsumerWrapper(consumer));

			//TODO: not pretty
			return step.Link.AttachToSource();
		}

		/// <summary>
		/// Defines a consumer for typed signal stream.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="step"></param>
		/// <param name="consumer">Consumer.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IConsumerOf<T> consumer)
		{
			step.Attach(new ConsumerWrapper<T>(consumer));

			//TODO: not pretty
			return step.Link.AttachToSource();
		}

		/// <summary>
		/// Defines a handler delegate for typed signal stream.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="step"></param>
		/// <param name="handler">Consumer delegate.</param>
		/// <param name="failureHandler">Failure handler delegate</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public static IDisposable ReactWith<T>(
			this ILinkJunction<T> step,
			Action<ISignal<T>> handler,
			Func<SignalException, bool> failureHandler = null)
		{
			step.Attach(new ConsumerWrapper<T>(handler, failureHandler));

			//TODO: not pretty
			return step.Link.AttachToSource();
		}

		/// <summary>
		/// Defines a handler delegate for untyped signal stream.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="handler">Consumer delegate.</param>
		/// <param name="failureHandler">Failure handler delegate</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public static IDisposable ReactWith(
			this ILinkJunction step,
			Action<ISignal> handler,
			Func<SignalException, bool> failureHandler = null)
		{
			step.Attach(new ConsumerWrapper(handler, failureHandler));

			//TODO: not pretty
			return step.Link.AttachToSource();
		}
	}
}