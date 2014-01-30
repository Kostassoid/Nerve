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

	internal sealed class Signal<T> : ISignal<T> where T : class
	{
		public T Body { get; private set; }
		object ISignal.Body { get { return Body; } }
		public StackTrace StackTrace { get; private set; }
		public ICell Sender { get { return StackTrace.Root; } }

		public Signal(T body, StackTrace stackTrace)
		{
			Body = body;
			StackTrace = stackTrace;
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			Sender.Fire(response);
		}

		public void Trace(ICell cell)
		{
			StackTrace.Push(cell);
		}

		private void Throw(Exception exception, IEmitter emitter)
		{
			var signalHandlingException =
				exception as SignalHandlingException
				?? new SignalHandlingException(exception, this);

			emitter.Fire(signalHandlingException);
		}

		public void ThrowOnAdjacent(Exception exception)
		{
			Throw(exception, StackTrace.Last);
		}

		public void ThrowOnSender(Exception exception)
		{
			Throw(exception, StackTrace.Root);
		}

		public ISignal<TOut> As<TOut>() where TOut : class
		{
			var body = Body as TOut;
			if (body == null)
				throw new InvalidCastException(string.Format("Unable to cast from [{0}] to [{1}].",
					typeof (T).Name, typeof (TOut).Name));

			return new Signal<TOut>(body, StackTrace);
		}
	}
}