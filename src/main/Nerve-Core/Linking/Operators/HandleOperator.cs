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

namespace Kostassoid.Nerve.Core.Linking.Operators
{
	using System;
	using Handling;
	using Signal;

	internal class HandleOperator : ILinkOperator
	{
		private readonly IReactiveHandler _reactiveHandler;

		public HandleOperator(IReactiveHandler reactiveHandler)
		{
			_reactiveHandler = reactiveHandler;
		}

		public void Process(ISignal signal)
		{
			try
			{
				_reactiveHandler.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalHandlingException(ex, signal);
				if (!_reactiveHandler.OnFailure(signalException))
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

	internal class HandleOperator<T> : ILinkOperator<T> where T : class
	{
		private readonly IReactiveHandlerOf<T> _reactiveHandler;

		public HandleOperator(IReactiveHandlerOf<T> reactiveHandler)
		{
			_reactiveHandler = reactiveHandler;
		}

		public void Process(ISignal<T> signal)
		{
			try
			{
				_reactiveHandler.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalHandlingException(ex, signal);
				if (!_reactiveHandler.OnFailure(signalException))
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