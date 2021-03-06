﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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
	using System.Linq;
	using Processing;
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
		public static ISignal<T> Of<T>(T payload)
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
		public static ISignal<T> Of<T>(T payload, IProcessor callback)
		{
			return new Signal<T>(payload, callback);
		}
	}

	internal sealed class Signal<T> : ISignal<T>
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

		public ISignal<TTarget> WithPayload<TTarget>(TTarget payload)
		{
			return new Signal<TTarget>(payload, Headers, Stacktrace, Callback);
		}

		public void MarkAsFaulted(Exception exception)
		{
			Requires.NotNull(exception, "exception");
			Requires.ValidState(
				!IsFaulted,
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

			if (Callback != null)
			{
				if (Callback.OnFailure(signalException))
				{
					return;
				}
			}

			Stacktrace.Frames
				.Where(p => p != Callback)
				.FirstOrDefault(p => p.OnFailure(signalException))
				.Ignore();
		}

		public void Return<TResponse>(TResponse response)
		{
			Requires.ValidState(Callback != null, "Callback receiver is not set on signal [{0}]", this);

			// ReSharper disable once PossibleNullReferenceException
			Callback.OnSignal(WithPayload(response));
		}

		public void Trace(IProcessor processor)
		{
			Stacktrace = Stacktrace.With(processor);
		}

		public ISignal<TOut> As<TOut>()
		{
			var s = this as ISignal<TOut>;
			if (s == null)
			{
				throw new InvalidCastException(string.Format("Expected [Signal of {0}] but got [{1}].", typeof(TOut).Name, GetType().BuildDescription()));
			}
			return s;
		}

		public ISignal<TOut> CastTo<TOut>()
		{
			//TODO: check performance
			//TODO: handle exception?
			return WithPayload((TOut)(object)Payload);
		}

		#endregion
	}
}