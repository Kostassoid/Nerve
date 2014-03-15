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
	using System.Linq;
	using Tools;
	using Tools.CodeContracts;

	internal sealed class Signal<T> : ISignal<T>
		where T : class
	{
		#region Constructors and Destructors

		public Signal(T payload, Headers headers, Stacktrace stacktrace)
		{
			Payload = payload;
			Headers = headers;
			Stacktrace = stacktrace;

			if (!stacktrace.Frames.IsEmpty)
			{
				Sender = stacktrace.Frames.Last();
			}
		}

		public Signal(T payload, Stacktrace stacktrace)
			: this(payload, Headers.Empty, stacktrace)
		{
		}

		#endregion

		#region Public Properties

		public Exception Exception { get; private set; }

		public Headers Headers { get; private set; }

		public T Payload { get; private set; }

		public ISignalProcessor Sender { get; private set; }

		public Stacktrace Stacktrace { get; private set; }

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

		public ISignal Clone()
		{
			return new Signal<T>(Payload, Headers.Clone(), Stacktrace.Clone());
		}

		public ISignal<TTarget> CloneWithPayload<TTarget>(TTarget payload) where TTarget : class
		{
			return new Signal<TTarget>(payload, Headers.Clone(), Stacktrace.Clone());
		}

		public void MarkAsFaulted(Exception exception)
		{
			Requires.ValidState(
				Exception == null,
				"Already marked as faulted with exception of type [{0}].",
				Exception != null ? Exception.GetType().Name : "");

			Exception = exception;
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			Sender.OnSignal(CloneWithPayload(response));
		}

		public void Trace(ISignalProcessor signalProcessor)
		{
			Stacktrace.Trace(signalProcessor);
		}

		public ISignal<TOut> CastTo<TOut>() where TOut : class
		{
			var s = this as ISignal<TOut>;
			if (s == null)
			{
				throw new InvalidCastException(string.Format("Expected [Signal of {0}] but got [{1}].", typeof(TOut).Name, GetType().BuildDescription()));
			}
			return s;
		}

		#endregion
	}
}