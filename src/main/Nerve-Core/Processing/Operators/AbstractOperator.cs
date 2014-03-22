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

namespace Kostassoid.Nerve.Core.Processing.Operators
{
	using Tools;

	public abstract class AbstractOperator : Processor, ILinkJunction
	{
		#region Fields

		private IProcessor _next;

		#endregion

		#region Constructors and Destructors

		protected AbstractOperator(ILink link)
		{
			Link = link;
		}

		#endregion

		#region Public Properties

		public IProcessor Next
		{
			get
			{
				return _next;
			}
		}

		public ILink Link { get; private set; }

		#endregion

		#region Public Methods and Operators

		public void Attach(IProcessor next)
		{
			_next = next;
		}

		public override string ToString()
		{
			return string.Format("Operator[{0}]", GetType().BuildDescription());
		}

		#endregion
	}

	public abstract class AbstractOperator<TIn, TOut> : AbstractOperator, ILinkJunction<TOut>
		where TIn : class where TOut : class
	{
		#region Constructors and Destructors

		protected AbstractOperator(ILink link)
			: base(link)
		{
		}

		#endregion

		#region Public Methods and Operators

		protected override void Process(ISignal signal)
		{
			InternalProcess(signal.As<TIn>());
		}

		public abstract void InternalProcess(ISignal<TIn> signal);

		#endregion
	}
}