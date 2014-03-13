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

namespace Kostassoid.Nerve.Core.Signal
{
	using System;

	using Kostassoid.Nerve.Core.Tools.CodeContracts;

	internal sealed class Signal<T> : ISignal<T> where T : class
	{
		public T Body { get; private set; }
		object ISignal.Body { get { return Body; } }
		public StackTrace StackTrace { get; private set; }
		public SignalException Exception { get; private set; }

		public ICell Sender { get { return StackTrace.Root; } }

		public Signal(T body, StackTrace stackTrace)
		{
			Body = body;
			StackTrace = stackTrace;
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			Sender.Fire(response);
		}

		public void Trace(ICell cell)
		{
			StackTrace.Push(cell);
		}

		public void HandleException(Exception exception)
		{
			Requires.ValidState(Exception == null, "Already marked as faulted with exception of type [{0}].", Exception != null ? Exception.GetType().Name : "");

			Exception = new SignalException(exception, this);
			Sender.OnFailure(Exception);
		}
	}
}