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
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Tools;
	using Tools.CodeContracts;

	public static class GateOp
	{
		public static ILinkJunction Gate(this ILinkJunction step, int threshold, TimeSpan timespan)
		{
			var next = new GateOperator(step.Link, threshold, timespan);
			step.Attach(next);
			return next;
		}

		public static ILinkJunction<T> Gate<T>(this ILinkJunction<T> step, int threshold, TimeSpan timespan) where T : class
		{
			var next = new GateOperator<T>(step.Link, threshold, timespan);
			step.Attach(next);
			return next;
		}

		internal class GateOperator : AbstractOperator
		{
			#region Fields

			private readonly int _threshold;

			private readonly ulong _timespan;

			private IList<UInt64> _ticks = new List<UInt64>();

			#endregion

			#region Constructors and Destructors

			public GateOperator(ILink link, int threshold, TimeSpan timespan)
				: base(link)
			{
				Requires.InRange(threshold >= 0, "threshold");
				Requires.InRange(timespan.TotalMilliseconds > 0, "timespan");

				_threshold = threshold;
				_timespan = (ulong)timespan.TotalMilliseconds;
			}

			#endregion

			#region Public Methods and Operators

			protected override void Process(ISignal signal)
			{
				ulong last = SystemTicks.Get();
				_ticks.Add(last);
				ulong first = _ticks[0];
				_ticks = _ticks.SkipWhile(t => last - first > _timespan).ToList();

				if (_ticks.Count >= _threshold)
				{
					Next.OnSignal(signal);
				}
			}

			#endregion
		}

		internal class GateOperator<T> : GateOperator, ILinkJunction<T>
			where T : class
		{
			#region Constructors and Destructors

			public GateOperator(ILink link, int threshold, TimeSpan timespan)
				: base(link, threshold, timespan)
			{
			}

			#endregion
		}
	}
}