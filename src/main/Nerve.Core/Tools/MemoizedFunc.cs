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

namespace Kostassoid.Nerve.Core.Tools
{
	using System;
	using System.Collections.Concurrent;

	internal static class MemoizedFunc
    {
        public static Func<TResult> From<TResult>(Func<TResult> func)
        {
			object cache = null;
			return () =>
			{
				if (cache == null)
					cache = func();

				return (TResult)cache;
			};
		}

		public static Func<TArg, TResult> From<TArg, TResult>(Func<TArg, TResult> func)
        {
			var cache = new ConcurrentDictionary<TArg, TResult>();
			return s => cache.GetOrAdd(s, _ => func(s));
		}

        public static Func<TArg1, TArg2, TResult> From<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> func)
        {
			var cache = new ConcurrentDictionary<Tuple<TArg1, TArg2>, TResult>();
			return (s1, s2) =>
			{
				var key = Tuple.Create(s1, s2);
				return cache.GetOrAdd(key, _ => func(s1, s2));
			};
		}

        public static Func<TArg1, TArg2, TArg3, TResult> From<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, TResult> func)
        {
			var cache = new ConcurrentDictionary<Tuple<TArg1, TArg2, TArg3>, TResult>();
			return (s1, s2, s3) =>
			{
				var key = Tuple.Create(s1, s2, s3);
				return cache.GetOrAdd(key, _ => func(s1, s2, s3));
			};
		}

        public static Func<TResult> AsMemoized<TResult>(this Func<TResult> func)
        {
	        return From(func);
        }

        public static Func<TArg, TResult> AsMemoized<TArg, TResult>(this Func<TArg, TResult> func)
        {
	        return From(func);
        }

        public static Func<TArg1, TArg2, TResult> AsMemoized<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
			return From(func);
		}

        public static Func<TArg1, TArg2, TArg3, TResult> AsMemoized<TArg1, TArg2, TArg3, TResult>(this Func<TArg1, TArg2, TArg3, TResult> func)
        {
			return From(func);
		}
    }
}