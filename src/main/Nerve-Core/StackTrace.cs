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
	using System.Collections.Generic;

	public class StackTrace
	{
		readonly IList<ICell> _stack = new List<ICell>();

		public StackTrace(ICell root)
		{
			_stack.Add(root);
		}

		protected StackTrace(IList<ICell> stack)
		{
			_stack = stack;
		}

		public ICell Root
		{
			get { return _stack[0]; }
		}

		public void Push(ICell cell)
		{
			_stack.Add(cell);
		}
	}
}