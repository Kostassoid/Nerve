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

namespace Kostassoid.Nerve.Core
{
	using System;

	using Signal;

	using Tools.CodeContracts;

	internal class LambdaHandlerOf<T> : IHandlerOf<T>
		where T : class
	{
		#region Fields

		private readonly Action<SignalException> _failureHandler;

		private readonly Action<ISignal<T>> _handler;

		#endregion

		#region Constructors and Destructors

		public LambdaHandlerOf(Action<ISignal<T>> handler, Action<SignalException> failureHandler)
		{
			Requires.NotNull(handler, "handler");

			_handler = handler;
			_failureHandler = failureHandler;
		}

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal<T> signal)
		{
			_handler(signal);
		}

		public bool OnFailure(SignalException exception)
		{
			if (_failureHandler != null)
			{
				_failureHandler(exception);
				return true;
			}

			return false;
		}

		#endregion
	}
}