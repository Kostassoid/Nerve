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
	using Signal;

	public abstract class Handler
	{
		public interface Base
		{
		}

		public interface OfFailure : Base
		{
			bool OnFailure(SignalException exception);
		}

		public interface Of<in T> : OfFailure
			where T : class
		{
			void OnSignal(ISignal<T> signal);
		}

		public interface OfAny : OfFailure
		{
			void OnSignal(ISignal signal);
		}
	}
}