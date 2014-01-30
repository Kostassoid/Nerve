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
using RabbitMQ.Client.Events;

namespace Kostassoid.Nerve.RabbitMq.Delivery
{
	public class IncomingDelivery
	{
		public RabbitChannel Channel { get; private set; }
		public BasicDeliverEventArgs Args { get; private set; }

		//TODO: find a better workaround
		private readonly bool _requiresAccept;
		private volatile bool _isAccepted;

		public string CorrelationId
		{
			get { return Args.BasicProperties.CorrelationId; }
		}

		public PublicationAddress ReplyToAddress
		{
			get { return Args.BasicProperties.ReplyTo != null
				? Args.BasicProperties.ReplyToAddress : null; }
		}

		private IDictionary<string, object> _headers;
		public IDictionary<string, object> Headers
		{
			get
			{
				if (_headers == null)
				{
					_headers = Args.BasicProperties.IsHeadersPresent()
						? Args.BasicProperties.Headers
						: new Dictionary<string, object>();
				}

				return _headers;
			}
		}

		public bool IsRequest
		{
			get { return CorrelationId != null && ReplyToAddress != null; }
		}

		public bool IsResponse
		{
			get { return CorrelationId != null && ReplyToAddress == null; }
		}

		public RabbitRoute IncomingRoute
		{
			get { return new RabbitRoute(Args.Exchange, Args.RoutingKey); }
		}

		public string ContentType
		{
			get { return Args.BasicProperties.ContentType; }
		}

		public IncomingDelivery(RabbitChannel channel, BasicDeliverEventArgs args, bool requiresAccept)
		{
			Channel = channel;
			Args = args;

			_requiresAccept = requiresAccept;
		}

		public void ReplyWith<T>(T message)
		{
			Channel.Reply(message, this);
		}

		public void Accept()
		{
			if (!_requiresAccept || _isAccepted) return;

			Channel.Accept(this);
			_isAccepted = true;
		}

		public void Reject(bool requeue)
		{
			if (!_requiresAccept || _isAccepted) return;

			Channel.Reject(this, requeue);
			_isAccepted = true;
		}

		public T UnpackAs<T>() where T : class
		{
			return Channel.UnpackAs<T>(this);
		}
	 
	}
}