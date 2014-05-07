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
	using Proxy;
	using Proxy.NProxy;

	/// <summary>
	/// Cell extension methods.
	/// </summary>
	public static class CellEx
	{
		private static readonly IProxyBuilder Builder = new NProxyBuilder();

		/// <summary>
		/// Builds a proxy object transforming method calls to <see cref="Invocation"/> messages.
		/// </summary>
		/// <typeparam name="T">Base class/interface type.</typeparam>
		/// <param name="cell">Cell to process invocation messages.</param>
		/// <returns>A new proxy object.</returns>
		public static T ProxyOf<T>(this ICell cell) where T : class
		{
			return Builder.Build<T>(cell);
		}
	}
}