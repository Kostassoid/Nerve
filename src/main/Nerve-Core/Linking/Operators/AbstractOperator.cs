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

namespace Kostassoid.Nerve.Core.Linking.Operators
{
	using System;
	using System.Linq;

	using Signal;

	public abstract class AbstractOperator : IHandler, ILinkJunction
	{
		#region Fields

		private IHandler _next;

		#endregion

		#region Constructors and Destructors

		protected AbstractOperator(ILink link)
		{
			Link = link;
		}

		#endregion

		#region Public Properties

		public IHandler Next
		{
			get
			{
				return _next;
			}
		}

		public ILink Link { get; private set; }

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal signal)
		{
			if (_next == null)
			{
				return;
			}

			try
			{
				signal.Trace(this);
				InternalProcess(signal);
			}
			catch (Exception ex)
			{
				var signalException = new SignalException(ex, signal);
				if (!_next.OnFailure(signalException))
				{
					signal.HandleException(signalException);
				}
			}
		}

		public virtual bool OnFailure(SignalException exception)
		{
			return false;
		}

		public void Attach(IHandler next)
		{
			_next = next;
		}

		public abstract void InternalProcess(ISignal signal);

		private string GetDescription()
		{
			var type = GetType();

			var name = type.Name;

			if (type.IsGenericType)
			{
				name = name.Remove(name.IndexOf('`'));
			}

			if (name.EndsWith("Operator"))
			{
				name = name.Substring(0, name.Length - "Operator".Length);
			}

			if (type.IsGenericType)
			{
				name += " of " + string.Join(",", type.GetGenericArguments().Select(t => t.Name));
				return name;
			}

			return name;
		}

		public override string ToString()
		{
			return string.Format("Operator[{0}]", GetDescription());
		}

		#endregion
	}

	public abstract class AbstractOperator<TIn, TOut> : AbstractOperator, IHandlerOf<TIn>, ILinkJunction<TOut>
		where TIn : class where TOut : class
	{
		#region Constructors and Destructors

		protected AbstractOperator(ILink link)
			: base(link)
		{
		}

		#endregion

		#region Public Methods and Operators

		public void OnSignal(ISignal<TIn> signal)
		{
			if (Next == null)
			{
				return;
			}

			InternalProcess(signal);
		}

		public override void InternalProcess(ISignal signal)
		{
			var s = signal as ISignal<TIn>;
			if (s == null)
			{
				return;
			}

			InternalProcess(s);
		}

		public abstract void InternalProcess(ISignal<TIn> signal);

		#endregion
	}
}