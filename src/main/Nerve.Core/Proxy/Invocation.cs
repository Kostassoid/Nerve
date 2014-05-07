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

namespace Kostassoid.Nerve.Core.Proxy
{
	using System;

	/// <summary>
	/// Method invocation description.
	/// </summary>
	[Serializable]
	public class Invocation
	{
		/// <summary>
		/// Method name.
		/// </summary>
		public string Method { get; private set; }

		/// <summary>
		/// Method invocation parameters.
		/// </summary>
		public object[] Params { get; private set; }

		/// <summary>
		/// Expected return type.
		/// </summary>
		public Type Expects { get; private set; }

		/// <summary>
		/// Initializes a new <see cref="Invocation"/> message.
		/// </summary>
		/// <param name="method">Method name.</param>
		/// <param name="expects">Expected return type.</param>
		/// <param name="params">Method invocation parameters.</param>
		public Invocation(string method, Type expects, params object[] @params)
		{
			Method = method;
			Expects = expects;
			Params = @params;
		}
	}
}