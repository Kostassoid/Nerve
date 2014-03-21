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

	using Processing;
	using Processing.Operators;
	using Tools;
	using Tools.CodeContracts;

	/// <summary>
	/// Static Signal builder.
	/// </summary>
	public static class Signal
	{
		/// <summary>
		/// Builds typed Signal from payload with empty headers and stacktrace.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="payload">Payload body.</param>
		/// <returns>New typed signal.</returns>
		public static ISignal<T> Of<T>(T payload) where T : class
		{
			return new Signal<T>(payload);
		}

		/// <summary>
		/// Builds typed Signal from payload with empty headers and stacktrace.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="payload">Payload body.</param>
		/// <param name="callback">Callback processor.</param>
		/// <returns>New typed signal.</returns>
		public static ISignal<T> Of<T>(T payload, IProcessor callback) where T : class
		{
			return new Signal<T>(payload, callback);
		}
	}

	internal sealed class Signal<T> : ISignal<T>
		where T : class
	{
		public Signal(T payload, Headers headers, Stacktrace stacktrace, IProcessor callback)
		{
			Payload = payload;
			Headers = headers;
			Stacktrace = stacktrace;
			Callback = callback;
		}

		public Signal(T payload, Headers headers, Stacktrace stacktrace)
			:this(payload, headers, stacktrace, null)
		{
		}

		public Signal(T payload, Stacktrace stacktrace)
			: this(payload, Headers.Empty, stacktrace)
		{
		}

		public Signal(T payload)
			: this(payload, Headers.Empty, Stacktrace.Empty)
		{
		}

		public Signal(T payload, IProcessor callback)
			: this(payload, Headers.Empty, Stacktrace.Empty, callback)
		{
		}

		public Exception Exception { get; private set; }

		public Headers Headers { get; private set; }

		public T Payload { get; private set; }

		public IProcessor Callback { get; private set; }

		public Stacktrace Stacktrace { get; private set; }

		public object this[string key]
		{
			get
			{
				return Headers[key];
			}
			set
			{
				Headers = value != null ? Headers.With(key, value) : Headers.Without(key);
			}
		}

		public bool IsFaulted
		{
			get { return Exception != null; }
		}

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
			//TODO: immutable signal perhaps?
			return new Signal<T>(Payload, Headers, Stacktrace, Callback);
		}

		public ISignal<TTarget> WithPayload<TTarget>(TTarget payload) where TTarget : class
		{
			return new Signal<TTarget>(payload, Headers, Stacktrace, Callback);
		}

		public void MarkAsFaulted(Exception exception)
		{
			Requires.ValidState(
				Exception == null,
				"Already marked as faulted with exception of type [{0}].",
				Exception != null ? Exception.GetType().Name : "");

			Exception = exception;

			HandleFailure();
		}

		void HandleFailure()
		{
			if (!IsFaulted)
			{
				return;
			}

			var signalException = new SignalException(Exception, this);
			var callbackIsNotified = false;
			foreach (var s in Stacktrace.Frames)
			{
				if (s == Callback)
				{
					callbackIsNotified = true;
				}

				if (s.OnFailure(signalException)) return;
			}

			if (Callback != null && !callbackIsNotified)
			{
				if (Callback.OnFailure(signalException))
				{
					return;
				}
			}
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			if (Callback == null)
			{
				throw new InvalidOperationException(string.Format("Callback receiver is not set on signal [{0}]", this));
			}

			Callback.OnSignal(WithPayload(response));
		}

		public void Trace(IProcessor processor)
		{
			Stacktrace = Stacktrace.With(processor);
		}

		public ISignal<TOut> As<TOut>() where TOut : class
		{
			var s = this as ISignal<TOut>;
			if (s == null)
			{
				throw new InvalidCastException(string.Format("Expected [Signal of {0}] but got [{1}].", typeof(TOut).Name, GetType().BuildDescription()));
			}
			return s;
		}

		public ISignal<TOut> CastTo<TOut>() where TOut : class
		{
			var p = Payload as TOut;
			if (p == null)
			{
				throw new InvalidCastException(string.Format("Expected payload of type [{0}] but got [{1}].", typeof(TOut).Name, Payload.GetType().Name));
			}
			return WithPayload(p);
		}

		#endregion
	}
}