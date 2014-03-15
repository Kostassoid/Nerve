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
	using Signal;
	using Tools;

	public abstract class SignalProcessor : ISignalProcessor
	{
		#region Public Methods and Operators

		public virtual void OnSignal(ISignal signal)
		{
			try
			{
				Process(signal);
			}
			catch (Exception ex)
			{
				signal.MarkAsFaulted(ex);
				var signalException = new SignalException(ex, signal);
				foreach (var s in signal.Stacktrace.Frames)
				{
					if (s.OnFailure(signalException)) return;
				}
			}
		}

		protected void Process(ISignal signal)
		{
			signal.Trace(this);
			InternalProcess(signal);
		}

		public virtual bool OnFailure(SignalException exception)
		{
			return false;
		}

		protected abstract void InternalProcess(ISignal signal);

		public override string ToString()
		{
			return string.Format("Processor[{0}]", GetType().BuildDescription());
		}

		#endregion
	}

}