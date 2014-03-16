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

	using Signal;

	public static class ReactOp
	{
		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, ISignalProcessor signalProcessor) where T : class
		{
			step.Attach(signalProcessor);

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(this ILinkJunction<T> step, IHandlerOf<T> handler) where T : class
		{
			step.Attach(new SignalHandlerWrapper<T>(handler));

			//TODO: not pretty
			return step.Link.AttachToCell();
		}

		public static IDisposable ReactWith<T>(
			this ILinkJunction<T> step,
			Action<ISignal<T>> handler,
			Func<SignalException, bool> failureHandler = null) where T : class
		{
			return ReactWith(step, new SignalHandlerWrapper<T>(handler, failureHandler));
		}
	}
}