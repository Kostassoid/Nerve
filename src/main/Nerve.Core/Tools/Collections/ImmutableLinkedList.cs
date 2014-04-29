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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	internal sealed class ImmutableLinkedList<T> : IImmutableLinkedList<T>
	{
		// ReSharper disable once InconsistentNaming
		readonly static ImmutableLinkedList<T> _empty = new ImmutableLinkedList<T>();
		readonly bool _isEmpty;
		readonly T _head;
		readonly ImmutableLinkedList<T> _tail;

		private ImmutableLinkedList()
		{
			_isEmpty = true;
		}

		private ImmutableLinkedList(T head)
		{
			_isEmpty = false;
			_head = head;
		}

		private ImmutableLinkedList(T head, ImmutableLinkedList<T> tail)
		{
			_isEmpty = false;
			_head = head;
			_tail = tail;
		}

		public static IImmutableLinkedList<T> Empty
		{
			get { return _empty; }
		}

		public int Count
		{
			get
			{
				var list = this;
				var count = 0;
				while (!list._isEmpty)
				{
					count++;
					list = list._tail;
				}
				return count;
			}
		}

		public bool IsEmpty
		{
			get { return _isEmpty; }
		}

		public T Head
		{
			get
			{
				if (_isEmpty)
					throw new InvalidOperationException("The list is empty.");
				return _head;
			}
		}

		public IImmutableLinkedList<T> Tail
		{
			get
			{
				if (_tail == null)
					throw new InvalidOperationException("This list has no tail.");
				return _tail;
			}
		}

		public static IImmutableLinkedList<T> FromEnumerable(IEnumerable<T> e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			return FromArrayInternal(e.ToArray());
		}

		public static IImmutableLinkedList<T> FromArray(params T[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			return FromArrayInternal(a);
		}

		public IImmutableLinkedList<T> Append(T value)
		{
			var array = new T[Count + 1];
			var list = this;
			var index = 0;
			while (!list._isEmpty)
			{
				array[index++] = list._head;
				list = list._tail;
			}
			array[index] = value;
			return FromArrayInternal(array);
		}

		public IImmutableLinkedList<T> Prepend(T value)
		{
			return new ImmutableLinkedList<T>(value, this);
		}

		public IImmutableLinkedList<T> Insert(int index, T value)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", "Cannot be less than zero.");
			var count = Count;
			if (index >= count)
				throw new ArgumentOutOfRangeException("index", "Cannot be greater than count.");
			var array = new T[Count + 1];
			var list = this;
			var arrayIndex = 0;
			while (!list._isEmpty)
			{
				if (arrayIndex == index)
				{
					array[arrayIndex++] = value;
				}
				array[arrayIndex++] = list._head;
				list = list._tail;
			}
			return FromArrayInternal(array);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var list = this;
			while (!list._isEmpty)
			{
				yield return list._head;
				list = list._tail;
			}
		}

		public override string ToString()
		{
			if (_isEmpty)
				return "[]";
			return string.Format("[{0}...]", _head);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		static IImmutableLinkedList<T> FromArrayInternal(T[] a)
		{
			var result = Empty;
			for (var i = a.Length - 1; i >= 0; i--)
			{
				result = result.Prepend(a[i]);
			}
			return result;
		}
	}
}