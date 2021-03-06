﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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

	using Tools;
	using Tools.CodeContracts;

	internal class ConsumerWrapper : Processor, IConsumerBase
	{
		#region Fields

		protected IConsumerBase Original { get; set; }

		protected readonly Func<SignalException, bool> FailureHandler;

		protected readonly Action<ISignal> Handler;

		#endregion

		#region Constructors and Destructors

		public ConsumerWrapper(Action<ISignal> handler, Func<SignalException, bool> failureHandler)
		{
			Requires.NotNull(handler, "processor");

			Handler = handler;
			FailureHandler = failureHandler;
			Original = this;
		}

		public ConsumerWrapper(IConsumer consumer):this(consumer.OnSignal, consumer.OnFailure)
		{
			Original = consumer;
		}

		#endregion

		#region Public Methods and Operators

		protected override void Process(ISignal signal)
		{
			//signal.With(this);
			Handler(signal);
		}

		public override bool OnFailure(SignalException exception)
		{
			if (FailureHandler == null) return false;

			return FailureHandler(exception);
		}

		public override string ToString()
		{
			return string.Format("Handler[{0}]", Original.GetType().BuildDescription());
		}

		#endregion
	}

	internal class ConsumerWrapper<T> : ConsumerWrapper
	{
		#region Constructors and Destructors

		public ConsumerWrapper(IConsumer consumer):base(consumer)
		{
		}

		public ConsumerWrapper(IConsumerOf<T> consumer)
			: base(s => consumer.OnSignal((Signal<T>)s), consumer.OnFailure)
		{
			Original = consumer;
		}

		public ConsumerWrapper(Action<ISignal<T>> handler, Func<SignalException, bool> failureHandler)
			: base(s => handler((Signal<T>)s), failureHandler)
		{
		}

		#endregion

		#region Public Methods and Operators

		public void Process(ISignal<T> signal)
		{
			base.Process(signal);
		}

		#endregion
	}
}