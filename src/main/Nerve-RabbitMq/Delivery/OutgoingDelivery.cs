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

using System.Collections.Generic;
using RabbitMQ.Client;

namespace Kostassoid.Nerve.RabbitMq.Delivery
{
	public class OutgoingDelivery
	{
		public string CorrelationId { get; private set; }
		public PublicationAddress ReplyToAddress { get; private set; }

		private IDictionary<string, object> _headers = new Dictionary<string, object>();
		public IDictionary<string, object> Headers { get { return _headers; } }

		public byte[] Body { get; private set; }

		public bool IsRequest
		{
			get { return CorrelationId != null && ReplyToAddress != null; }
		}

		public bool IsResponse
		{
			get { return CorrelationId != null && ReplyToAddress == null; }
		}

		public RabbitRoute OutgoingRoute { get; private set; }

		public string ContentType { get; private set; }

		public OutgoingDelivery()
		{
		}
	}
}