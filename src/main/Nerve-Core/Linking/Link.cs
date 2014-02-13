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

namespace Kostassoid.Nerve.Core.Linking
{
	using System;
	using Operators;
	using Signal;

	internal class Link : ILink
	{
		private readonly Cell _owner;
		private readonly RootOperator _root;

		public Link(Cell owner)
		{
			_owner = owner;
			_root = new RootOperator(this);
		}

		public ILinkContinuation Root { get { return _root; } }

		public void Process(ISignal signal)
		{
			_root.Process(signal);
		}

		public IDisposable AttachToCell()
		{
			return _owner.Attach(this);
		}

		public static ILinkContinuation OnStream()
		{

			throw new NotImplementedException();
		}
	}
}