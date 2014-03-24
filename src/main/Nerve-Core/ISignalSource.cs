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
	/// Signal source.
	/// </summary>
	public interface ISignalSource
	{
		/// <summary>
		/// Attaching (subscribing) processor to this signal source.
		/// </summary>
		/// <param name="processor">Processor to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		IDisposable Attach(IProcessor processor);

		//TODO: extract
		/// <summary>
		/// Attaching (subscribing) untyped consumer to this signal source.
		/// </summary>
		/// <param name="consumer">Consumer to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		IDisposable Attach(IConsumer consumer);

		/// <summary>
		/// Attaches (subscribes) typed consumer to stream event.
		/// </summary>
		/// <param name="consumer">Consumer to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		IDisposable Attach<T>(IConsumerOf<T> consumer) where T : class;

		/// <summary>
		/// Starts building signal stream processing chain.
		/// </summary>
		/// <returns>Link extending point.</returns>
		ILinkJunction OnStream();
	}
}