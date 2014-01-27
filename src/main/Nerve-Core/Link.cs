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
	using Pipeline;
	using Signal;

	public class Link : IDisposable
	{
		public IAgent Owner { get; private set; }
		public IPipelineStep Pipeline { get; private set; }

		public Link(IAgent owner)
		{
			Pipeline = new PipelineStep<object>(this);
			Owner = owner;
		}

		public void Process(ISignal signal)
		{
			Pipeline.Execute(signal);
		}

		public void Subscribe()
		{
			Owner.Subscribe(this);
		}

		public void Dispose()
		{
			Owner.Unsubscribe(this);
		}
	}
}