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

	public static class ReactOp
	{
		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IProcessor processor) where T : class
		{
			step.Attach(processor);

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IConsumerOf<T> consumer) where T : class
		{
			step.Attach(new ConsumerWrapper<T>(consumer));

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(
			this ILinkJunction<T> step,
			Action<ISignal<T>> handler,
			Func<SignalException, bool> failureHandler = null) where T : class
		{
			step.Attach(new ConsumerWrapper<T>(handler, failureHandler));

			//TODO: not pretty
			return step.Link.AttachToCell();
		}
	}
}