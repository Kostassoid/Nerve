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
	using Processing;

	/// <summary>
	/// Untyped link extension point.
	/// </summary>
	public interface ILinkJunction
	{
		/// <summary>
		/// Underlying link.
		/// </summary>
		ILink Link { get; }

		/// <summary>
		/// Attaches processor to the processing chain.
		/// </summary>
		/// <param name="next">Processor to attach.</param>
		void Attach(IProcessor next);
	}

	/// <summary>
	/// Typed link extension point.
	/// </summary>
	public interface ILinkJunction<out T> : ILinkJunction
	{
	}
}