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

	/// <summary>
	/// Base abstract untyped operator.
	/// </summary>
	public abstract class AbstractOperator : Processor, ILinkJunction
	{
		private IProcessor _next;

		/// <summary>
		/// Builds new operator.
		/// </summary>
		/// <param name="link">Link to attach to.</param>
		protected AbstractOperator(ILink link)
		{
			Link = link;
		}

		/// <summary>
		/// Next processor in processing chain.
		/// </summary>
		public IProcessor Next
		{
			get
			{
				return _next;
			}
		}

		/// <summary>
		/// Owner link.
		/// </summary>
		public ILink Link { get; private set; }

		/// <summary>
		/// Sets next processor in chain.
		/// </summary>
		/// <param name="next"></param>
		public void Attach(IProcessor next)
		{
			_next = next;
		}

		/// <summary>
		/// Builds readable operator description.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Operator[{0}]", GetType().BuildDescription());
		}
	}

	/// <summary>
	/// Base abstract typed operator.
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <typeparam name="TOut"></typeparam>
	public abstract class AbstractOperator<TIn, TOut> : AbstractOperator, ILinkJunction<TOut>
		where TIn : class where TOut : class
	{
		/// <summary>
		/// Builds new operator.
		/// </summary>
		/// <param name="link">Link to attach to.</param>
		protected AbstractOperator(ILink link)
			: base(link)
		{
		}

		/// <summary>
		/// Processes incoming signal.
		/// </summary>
		/// <param name="signal"></param>
		protected override void Process(ISignal signal)
		{
			InternalProcess(signal.As<TIn>());
		}

		/// <summary>
		/// Performs operator-specific signal processing.
		/// </summary>
		/// <param name="signal"></param>
		protected abstract void InternalProcess(ISignal<TIn> signal);
	}
}