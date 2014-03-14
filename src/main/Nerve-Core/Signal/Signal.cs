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

	using Tools.CodeContracts;

	internal sealed class Signal<T> : ISignal<T>
		where T : class
	{
		#region Constructors and Destructors

		public Signal(T payload, Headers headers, StackTrace stackTrace)
		{
			Payload = payload;
			Headers = headers;
			StackTrace = stackTrace;
		}

		public Signal(T payload, StackTrace stackTrace)
			: this(payload, Headers.Empty, stackTrace)
		{
		}

		#endregion

		#region Public Properties

		public SignalException Exception { get; private set; }

		public Headers Headers { get; private set; }

		public T Payload { get; private set; }

		public ICell Sender
		{
			get
			{
				return StackTrace.Root;
			}
		}

		public StackTrace StackTrace { get; private set; }

		#endregion

		#region Explicit Interface Properties

		object ISignal.Payload
		{
			get
			{
				return Payload;
			}
		}

		#endregion

		#region Public Methods and Operators

		public ISignal<TTarget> CloneWithPayload<TTarget>(TTarget payload) where TTarget : class
		{
			return new Signal<TTarget>(payload, Headers.Clone(), StackTrace.Clone());
		}

		public void HandleException(Exception exception)
		{
			Requires.ValidState(
				Exception == null,
				"Already marked as faulted with exception of type [{0}].",
				Exception != null ? Exception.GetType().Name : "");

			Exception = new SignalException(exception, this);
			Sender.OnFailure(Exception);
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			Sender.Fire(response);
		}

		public void Trace(ICell cell)
		{
			StackTrace.Push(cell);
		}

		#endregion
	}
}