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
	using Signal;

	internal class HandleOperator : ILinkOperator
	{
		private readonly IConsumer _consumer;

		public HandleOperator(IConsumer consumer)
		{
			_consumer = consumer;
		}

		public void Process(ISignal signal)
		{
			try
			{
				_consumer.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalException(ex, signal);
				if (!_consumer.OnFailure(signalException))
				{
					signal.HandleException(signalException);
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
		private readonly IConsumerOf<T> _consumer;

		public HandleOperator(IConsumerOf<T> consumer)
		{
			_consumer = consumer;
		}

		public void Process(ISignal<T> signal)
		{
			try
			{
				_consumer.Handle(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalException(ex, signal);
				if (!_consumer.OnFailure(signalException))
				{
					signal.HandleException(signalException);
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