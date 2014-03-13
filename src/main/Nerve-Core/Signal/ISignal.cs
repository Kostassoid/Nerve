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

using System;

namespace Kostassoid.Nerve.Core.Signal
{
	/// <summary>
	/// Untyped signal interface
	/// </summary>
	public interface ISignal
	{
		/// <summary>
		/// Signal payload.
		/// </summary>
		object Body { get; }

		/// <summary>
		/// Original signal sender.
		/// </summary>
		ICell Sender { get; }

		/// <summary>
		/// Recorded stack trace.
		/// </summary>
		StackTrace StackTrace { get; }

		/// <summary>
		/// First exception encountered during signal processing.
		/// </summary>
		SignalException Exception { get; }

		/// <summary>
		/// Fires back at original sender.
		/// </summary>
		/// <typeparam name="TResponse">Response payload type.</typeparam>
		/// <param name="body">Signal payload.</param>
		void Return<TResponse>(TResponse body) where TResponse : class;

		/// <summary>
		/// Registers intermediate cell in stack trace.
		/// </summary>
		/// <param name="cell">Cell</param>
		void Trace(ICell cell);

		/// <summary>
		/// Handles exception.
		/// </summary>
		/// <param name="exception">Original exception.</param>
		void HandleException(Exception exception);
	}

	/// <summary>
	/// Typed signal interface.
	/// </summary>
	/// <typeparam name="T">Signal payload type.</typeparam>
	public interface ISignal<out T> : ISignal where T : class
	{
		/// <summary>
		/// Signal payload.
		/// </summary>
		new T Body { get; }
	}
}