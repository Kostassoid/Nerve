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
	using Tools;
	using Tools.CodeContracts;

	internal class SignalHandlerWrapper : ISignalProcessor, IHandlerBase
	{
		#region Fields

		protected IHandlerBase Original { get; set; }

		protected readonly Func<SignalException, bool> FailureHandler;

		protected readonly Action<ISignal> Handler;

		#endregion

		#region Constructors and Destructors

		protected SignalHandlerWrapper(Action<ISignal> handler, Func<SignalException, bool> failureHandler)
		{
			Requires.NotNull(handler, "signalProcessor");

			Handler = handler;
			FailureHandler = failureHandler;
			Original = this;
		}

		public SignalHandlerWrapper(IHandler handler):this(handler.OnSignal, handler.OnFailure)
		{
			Original = handler;
		}

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal signal)
		{
			signal.Trace(this);
			Handler(signal);
		}

		public bool OnFailure(SignalException exception)
		{
			if (FailureHandler == null) return false;

			return FailureHandler(exception);
		}

		public override string ToString()
		{
			return string.Format("Handler[{0}]", Original.GetType().BuildDescription());
		}

		#endregion
	}

	internal class SignalHandlerWrapper<T> : SignalHandlerWrapper
		where T : class
	{
		#region Constructors and Destructors

		public SignalHandlerWrapper(IHandler handler):base(handler)
		{
		}

		public SignalHandlerWrapper(IHandlerOf<T> handler)
			: base(s => handler.OnSignal((Signal<T>)s), handler.OnFailure)
		{
			Original = handler;
		}

		public SignalHandlerWrapper(Action<ISignal<T>> handler, Func<SignalException, bool> failureHandler)
			: base(s => handler((Signal<T>)s), failureHandler)
		{
		}

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal<T> signal)
		{
			OnSignal(signal as ISignal);
		}

		#endregion
	}
}