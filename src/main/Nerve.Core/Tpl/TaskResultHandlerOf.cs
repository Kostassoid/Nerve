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

namespace Kostassoid.Nerve.Core.Tpl
{
	using System.Threading.Tasks;
	using Core;
	using Processing;

	/// <summary>
	/// Task based signal processor.
	/// </summary>
	/// <typeparam name="T">Signal payload type.</typeparam>
	public class TaskResultHandlerOf<T> : Processor, ITaskResultHandler where T : class
	{
		readonly TaskCompletionSource<T> _completionSource = new TaskCompletionSource<T>();

		/// <summary>
		/// Task to await for signal.
		/// </summary>
		public Task Task
		{
			get
			{
				return _completionSource.Task;
			}
		}

		public IProcessor Processor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Task to await for signal.
		/// </summary>
		public Task<T> TypedTask
		{
			get
			{
				return _completionSource.Task;
			}
		}

		/// <summary>
		/// Handles processing failure. Sets underlying Task exception.
		/// </summary>
		/// <param name="exception">Wrapped exception.</param>
		/// <returns></returns>
		public override bool OnFailure(SignalException exception)
		{
			_completionSource.SetException(exception.InnerException);
			return true;
		}

		/// <summary>
		/// Handles incoming signal. Sets underlying Task result.
		/// </summary>
		/// <param name="signal">Signal to process.</param>
		/// <returns></returns>
		protected override void Process(ISignal signal)
		{
			_completionSource.SetResult(signal.CastTo<T>().Payload);
		}
	}
}