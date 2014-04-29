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
	using System.Collections.Generic;
	using System.Linq.Expressions;

	internal static class New
	{
		private static readonly IDictionary<Type, Func<object>> Cache
 			= new Dictionary<Type, Func<object>>();

		public static object InstanceOf(Type type)
		{
			Func<object> factory;
			if (!Cache.TryGetValue(type, out factory))
			{
				factory = Expression
				.Lambda<Func<object>>(Expression.New(type))
				.Compile();

				Cache[type] = factory;
			}

			return factory();
		}
	}
}