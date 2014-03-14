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

	internal class HandlerConnector<T> : IHandler, IHandlerOf<T>
		where T : class
	{
		#region Fields

		private readonly Func<SignalException, bool> _failureHandler;

		private readonly Action<ISignal> _handler;

		#endregion

		#region Constructors and Destructors

		public HandlerConnector(IHandler handler)
		{
			Requires.NotNull(handler, "handler");

			_handler = handler.OnSignal;
			_failureHandler = handler.OnFailure;
		}

		public HandlerConnector(IHandlerOf<T> handler)
		{
			Requires.NotNull(handler, "handler");

			_handler = s => handler.OnSignal((Signal<T>)s);
			_failureHandler = handler.OnFailure;
		}

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal signal)
		{
			_handler(signal);
		}

		public bool OnFailure(SignalException exception)
		{
			return _failureHandler(exception);
		}

		public void OnSignal(ISignal<T> signal)
		{
			_handler(signal);
		}

		#endregion
	}
}