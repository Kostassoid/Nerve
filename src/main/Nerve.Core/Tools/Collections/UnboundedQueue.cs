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
	using System.Collections.Generic;
	using System.Threading;

	public class UnboundedQueue<T> : IQueue<T>
	{
		Queue<T> _primary = new Queue<T>();
		Queue<T> _secondary = new Queue<T>();
		readonly object _sync1 = new object();
		readonly object _sync2 = new object();
		int _closed;
		int _count;

		public int Count
		{
			get { return _count; }
		}

		public void Enqueue(T item)
		{
			lock (_sync1)
			{
				_primary.Enqueue(item);
			}

			Interlocked.Increment(ref _count);
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
			lock (_sync2)
			{
				_secondary = Interlocked.Exchange(ref _primary, _secondary);

				var count = _secondary.Count;
				while (_secondary.Count > 0)
				{
					yield return _secondary.Dequeue();
				}

				Interlocked.Add(ref _count, -count);
			}
		}
	}
}