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

	/// <summary>
	///   Untyped signal interface
	/// </summary>
	public interface ISignal
	{
		#region Public Properties

		/// <summary>
		///   First exception encountered during signal processing.
		/// </summary>
		SignalException Exception { get; }

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
		IHandler Sender { get; }

		/// <summary>
		///   Recorded stack trace.
		/// </summary>
		StackTrace StackTrace { get; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///   Clone current signal using new payload.
		/// </summary>
		/// <typeparam name="TTarget">New payload type.</typeparam>
		/// <param name="payload">New payload</param>
		/// <returns>Newly created signal.</returns>
		ISignal<TTarget> CloneWithPayload<TTarget>(TTarget payload) where TTarget : class;

		/// <summary>
		///   Handles exception.
		/// </summary>
		/// <param name="exception">Original exception.</param>
		void HandleException(Exception exception);

		/// <summary>
		///   Fires back at original sender.
		/// </summary>
		/// <typeparam name="TResponse">Response payload type.</typeparam>
		/// <param name="body">Signal payload.</param>
		void Return<TResponse>(TResponse body) where TResponse : class;

		/// <summary>
		///   Registers intermediate handler in stack trace.
		/// </summary>
		/// <param name="handler">Handler</param>
		void Trace(IHandler handler);

		#endregion
	}

	/// <summary>
	///   Typed signal interface.
	/// </summary>
	/// <typeparam name="T">Signal payload type.</typeparam>
	public interface ISignal<out T> : ISignal
		where T : class
	{
		#region Public Properties

		/// <summary>
		///   Signal payload.
		/// </summary>
		new T Payload { get; }

		#endregion
	}
}