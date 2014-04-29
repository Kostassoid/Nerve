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

	/// <summary>
	///   Untyped signal interface
	/// </summary>
	public interface ISignal
	{
		/// <summary>
		///   First exception encountered during signal processing.
		/// </summary>
		Exception Exception { get; }

		/// <summary>
		///   Signal headers.
		/// </summary>
		Headers Headers { get; }

		/// <summary>
		///   Signal payload.
		/// </summary>
		object Payload { get; }

		/// <summary>
		///   Original signal sender.
		/// </summary>
		IProcessor Callback { get; }

		/// <summary>
		///   Recorded stack trace.
		/// </summary>
		Stacktrace Stacktrace { get; }

		/// <summary>
		/// Returns headers value using key, or null if not present.
		/// </summary>
		/// <param name="key">Header key</param>
		/// <returns></returns>
		object this[string key] { get; set; }

		/// <summary>
		/// Clones this signal.
		/// </summary>
		/// <returns></returns>
		ISignal Clone();

		/// <summary>
		///   Clone current signal using new payload.
		/// </summary>
		/// <typeparam name="TTarget">New payload type.</typeparam>
		/// <param name="payload">New payload</param>
		/// <returns>Newly created signal.</returns>
		ISignal<TTarget> WithPayload<TTarget>(TTarget payload);

		/// <summary>
		///   Handles exception.
		/// </summary>
		/// <param name="exception">Original exception.</param>
		void MarkAsFaulted(Exception exception);

		/// <summary>
		/// Sends response message to callback handler.
		/// </summary>
		/// <typeparam name="TResponse">Response payload type.</typeparam>
		/// <param name="body">Signal payload.</param>
		void Return<TResponse>(TResponse body);

		/// <summary>
		///   Registers intermediate processor in stack trace.
		/// </summary>
		/// <param name="processor">Handler</param>
		void Trace(IProcessor processor);

		/// <summary>
		/// Casts payload to another type. Throws in case of type mismatch.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <returns>Typed signal.</returns>
		ISignal<T> CastTo<T>();

		/// <summary>
		/// Casts to typed signal. Throws in case of type mismatch.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <returns>Typed signal.</returns>
		ISignal<T> As<T>();
	}

	/// <summary>
	///   Typed signal interface.
	/// </summary>
	/// <typeparam name="T">Signal payload type.</typeparam>
	public interface ISignal<out T> : ISignal
	{
		/// <summary>
		///   Signal payload.
		/// </summary>
		new T Payload { get; }
	}
}