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

	/// <summary>
	///   SignalException handler delegate type.
	/// </summary>
	/// <param name="cell">Cell which raised an exception.</param>
	/// <param name="exception">Wrapped exception.</param>
	public delegate void SignalExceptionHandler(ICell cell, SignalException exception);

	/// <summary>
	///   Base Cell interface.
	/// </summary>
	public interface ICell : ISignalSource, ISignalProcessor, IDisposable
	{
		#region Public Events

		/// <summary>
		///   Unhandled exception event.
		/// </summary>
		event SignalExceptionHandler Failed;

		#endregion

		#region Public Properties

		/// <summary>
		///   Cell name.
		/// </summary>
		string Name { get; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///   Creates a new signal and schedules its execution on current Cell.
		/// </summary>
		/// <typeparam name="T">Signal payload type.</typeparam>
		/// <param name="body">Signal payload body.</param>
		void Fire<T>(T body) where T : class;

		#endregion
	}
}