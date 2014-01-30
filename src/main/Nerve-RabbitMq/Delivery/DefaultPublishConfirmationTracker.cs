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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kostassoid.Nerve.Core.Tools;

namespace Kostassoid.Nerve.RabbitMq.Delivery
{
	internal class DefaultPublishConfirmationTracker : IPublishConfirmationTracker
	{
		private readonly RabbitChannel _channel;
		private readonly IDictionary<ulong, TaskCompletionSource<object>> _pending = new ConcurrentDictionary<ulong, TaskCompletionSource<object>>();

		public DefaultPublishConfirmationTracker(RabbitChannel channel)
		{
			_channel = channel;
		}

		public Task Track()
		{
			var completionSource = new TaskCompletionSource<object>();
			_pending.Add(_channel.GetNextSeqNo(), completionSource);
			return completionSource.Task;
		}

		public void HandleConfirmation(bool isConfirmed, ulong seqNo, bool isMultiple)
		{
			if (isMultiple)
			{
				_pending.Keys.Where(r => r <= seqNo)
					.ToArray()
					.ForEach(k => ProcessConfirmation(k, isConfirmed));
			}
			else
			{
				ProcessConfirmation(seqNo, isConfirmed);
			}
		}

		private void ProcessConfirmation(ulong seqNo, bool isConfirmed)
		{
			TaskCompletionSource<object> completionSource;
			if (_pending.TryGetValue(seqNo, out completionSource))
			{
				if (_pending.Remove(seqNo))
				{
					if (isConfirmed)
						completionSource.TrySetResult(null);
					else
						completionSource.TrySetException(new MessageRejectedException());
				}
			}
		}

		public void Reset()
		{
			if (_pending == null) return;

			_pending.Values.ForEach(v => v.TrySetException(new MessageRejectedException()));
			_pending.Clear();
		}

		public void Dispose()
		{
			Reset();
		}
	}
}
