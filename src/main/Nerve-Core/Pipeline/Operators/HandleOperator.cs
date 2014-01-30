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
using Kostassoid.Nerve.Core.Signal;

namespace Kostassoid.Nerve.Core.Pipeline.Operators
{
	internal class HandleOperator : ISynapseOperator
	{
		private readonly IHandler _handler;

		public HandleOperator(IHandler handler)
		{
			_handler = handler;
		}

		public void Process(ISignal signal)
		{
			try
			{
				_handler.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalHandlingException(ex, signal);
				if (!_handler.OnFailure(signalException))
				{
					signal.ThrowOnAdjacent(signalException);
				}
			}
		}

		public override string ToString()
		{
			return string.Format("Operator [{0}]", GetType().Name);
		}
	}

	internal class HandleOperator<T> : ISynapseOperator<T> where T : class
	{
		private readonly IHandlerOf<T> _handler;

		public HandleOperator(IHandlerOf<T> handler)
		{
			_handler = handler;
		}

		public void Process(ISignal<T> signal)
		{
			try
			{
				_handler.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalHandlingException(ex, signal);
				if (!_handler.OnFailure(signalException))
				{
					signal.ThrowOnAdjacent(signalException);
				}
			}
		}

		public void Process(ISignal signal)
		{
			Process(signal as ISignal<T>);
		}

		public override string ToString()
		{
			return string.Format("Operator [{0}]", GetType().Name);
		}
	}
}