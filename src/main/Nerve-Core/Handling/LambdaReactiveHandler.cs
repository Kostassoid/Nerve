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

namespace Kostassoid.Nerve.Core.Handling
{
    using System;
    using Kostassoid.Nerve.Core.Signal;
    using Kostassoid.Nerve.Core.Tools.CodeContracts;

    internal class LambdaReactiveHandler<T> : IReactiveHandlerOf<T> where T : class
	{
		readonly Action<ISignal<T>> _handler;
		readonly Action<SignalHandlingException> _failureHandler;

		public LambdaReactiveHandler(Action<ISignal<T>> handler, Action<SignalHandlingException> failureHandler)
		{
			Requires.NotNull(handler, "handler");

			_handler = handler;
			_failureHandler = failureHandler;
		}

		public void Handle(ISignal<T> signal)
		{
			_handler(signal);
		}

		public bool OnFailure(SignalHandlingException exception)
		{
			if (_failureHandler != null)
			{
				_failureHandler(exception);
				return true;
			}

			return false;
		}
	}
}