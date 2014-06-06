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

namespace Kostassoid.Nerve.Core.Tools.Collections
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;

	public class UnboundedQueue<T> : IQueue<T>
	{
		Queue<T> _primary = new Queue<T>();
		Queue<T> _secondary = new Queue<T>();
		readonly object _sync = new object();
		int _closed;

		public int Count
		{
			get
			{
				lock (_sync)
				{
					return _primary.Count;
				}
			}
		}

		public void Enqueue(T item)
		{
			lock (_sync)
			{
				_primary.Enqueue(item);
			}
		}

/*
		public bool TryDequeue(out T item)
		{
			lock (_sync)
			{
				if (_primary.Count == 0)
				{
					item = default(T);
					return false;
				}

				item = _primary.Dequeue();
				return true;
			}
		}
*/

		public IEnumerable<T> DequeueAll()
		{
			_secondary = Interlocked.Exchange(ref _primary, _secondary);

			while (_secondary.Count > 0)
			{
				yield return _secondary.Dequeue();
			}
		}
	}
}