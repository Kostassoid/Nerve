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
	using Signal;

	internal class EmitterOf<T> : IEmitterOf<T> where T : class
	{
		readonly ICell _cell;

		public EmitterOf(ICell cell)
		{
			_cell = cell;
		}

		public void Fire(T body)
		{
			_cell.Fire(body);
		}

		public void Fire(ISignal<T> signal)
		{
			_cell.Fire(signal);
		}
	}
}