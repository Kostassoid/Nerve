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

namespace Kostassoid.Nerve.Core.Pipeline
{
	using System;
	using Scheduling;
	using Signal;

	internal class PipelineStep<T> : IPipelineStep<T> where T : class
	{
		Action<ISignal<T>> _next;

		public Link Link { get; private set; }

		public PipelineStep(Link link)
		{
			Link = link;
		}

		public void Execute(ISignal<T> item)
		{
			if (_next == null) return;
			_next(item);
		}

		public void Attach(Action<ISignal<T>> action)
		{
			_next = action;
		}

		public void ScheduleOn(IScheduler scheduler)
		{
			Link.Scheduler = scheduler;
		}

		void IPipelineStep.Execute(ISignal signal)
		{
			Execute(signal.As<T>());
		}

		void IPipelineStep.Attach(Action<ISignal> action)
		{
			_next = action;
		}
	}
}