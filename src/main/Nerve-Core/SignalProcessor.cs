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
	using Tools;

	/// <summary>
	/// Base signal processor code.
	/// </summary>
	public abstract class SignalProcessor : ISignalProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Signal processing entry point.
		/// </summary>
		/// <param name="signal"></param>
		public virtual void OnSignal(ISignal signal)
		{
			try
			{
				signal.Trace(this);
				Process(signal);
			}
			catch (Exception ex)
			{
				signal.MarkAsFaulted(ex);
				var signalException = new SignalException(ex, signal);

				if (OnFailure(signalException))
				{
					return;
				}

				if (signal.Callback != null)
				{
					if (signal.Callback.OnFailure(signalException))
					{
						return;
					}
				}

				foreach (var s in signal.Stacktrace.Frames)
				{
					//if (s == signal.Callback) continue;

					if (s.OnFailure(signalException)) return;
				}
				//throw signalException;
			}
		}

		/// <summary>
		/// Processing failure handler.
		/// </summary>
		/// <param name="exception">Wrapped exception.</param>
		/// <returns>True if exception was handled.</returns>
		public virtual bool OnFailure(SignalException exception)
		{
			return false;
		}

		/// <summary>
		/// Processor logic.
		/// </summary>
		/// <param name="signal"></param>
		protected abstract void Process(ISignal signal);

		public override string ToString()
		{
			return string.Format("Processor[{0}]", GetType().BuildDescription());
		}

		#endregion
	}

}