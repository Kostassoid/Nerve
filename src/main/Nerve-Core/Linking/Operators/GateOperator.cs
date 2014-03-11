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
	using System.Collections.Generic;
	using System.Linq;
	using Signal;
	using Tools;
	using Tools.CodeContracts;

	internal class GateOperator : AbstractOperator
	{
		private readonly int _threshold;
		private readonly ulong _timespan;
		private IList<UInt64> _ticks = new List<UInt64>();

		public GateOperator(ILink link, int threshold, TimeSpan timespan)
			: base(link)
		{
			Requires.InRange(threshold >= 0, "threshold");
			Requires.InRange(timespan.TotalMilliseconds > 0, "timespan");

			_threshold = threshold;
			_timespan = (ulong)timespan.TotalMilliseconds;
		}

		public override void InternalProcess(ISignal signal)
		{
			var last = SystemTicks.Get();
			_ticks.Add(last);
			var first = _ticks[0];
			_ticks = _ticks.SkipWhile(t => last - first > _timespan).ToList();

			if (_ticks.Count >= _threshold)
				Next.Process(signal);
		}
	}

	internal class GateOperator<T> : GateOperator/*, ILinkContinuation<T>*/ where T : class
	{
		public GateOperator(ILink link, int threshold, TimeSpan timespan)
			: base(link, threshold, timespan)
		{}

		public void Attach(ILinkOperator<T> next)
		{
			base.Attach(next);
		}
	}

}