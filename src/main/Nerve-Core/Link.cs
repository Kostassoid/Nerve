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
	using System;

	using Processing.Operators;

	internal class Link : ILink
	{
		#region Fields

		private readonly ISignalSource _owner;

		private readonly RootOperator _root;

		#endregion

		#region Constructors and Destructors

		public Link(ISignalSource owner)
		{
			_owner = owner;
			_root = new RootOperator(this);
		}

		#endregion

		#region Public Properties

		public ILinkJunction Root
		{
			get
			{
				return _root;
			}
		}

		#endregion

		#region Public Methods and Operators

		public IDisposable AttachToCell()
		{
			return _owner.Attach(this);
		}

		public void OnSignal(ISignal signal)
		{
			_root.OnSignal(signal);
		}

		public bool OnFailure(SignalException exception)
		{
			return false;
		}

		#endregion
	}
}