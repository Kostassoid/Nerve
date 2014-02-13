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
	using Signal;

	internal class FilterOperator<T> : AbstractOperator<T, T>
		where T : class
	{
		private readonly Func<T, bool> _predicateFunc;

		public FilterOperator(ILink link, Func<T, bool> predicateFunc) : base(link)
		{
			_predicateFunc = predicateFunc;
		}

		public override void InternalProcess(ISignal<T> signal)
		{
			if (!_predicateFunc(signal.Body)) return;
			Next.Process(signal);
		}
	}

	internal class FilterOperator : AbstractOperator
	{
		private readonly Func<object, bool> _predicateFunc;

		public FilterOperator(ILink link, Func<object, bool> predicateFunc)
			: base(link)
		{
			_predicateFunc = predicateFunc;
		}

		public override void InternalProcess(ISignal signal)
		{
			if (!_predicateFunc(signal.Body)) return;
			Next.Process(signal);
		}
	}
}