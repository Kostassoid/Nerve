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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Kostassoid.Nerve.RabbitMq.Configuration.Topology;
using Kostassoid.Nerve.RabbitMq.Delivery;
using Kostassoid.Nerve.RabbitMq.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Kostassoid.Nerve.RabbitMq
{
	public class RabbitChannel
	{
		public event Action<RabbitChannel, ErrorEventArgs> Failed = (channel, args) => { };

		protected IModel Native { get; private set; }

		private readonly IMessageSerializer _serializer;
		private bool _isClosed;

		public RabbitChannel(IModel native, IMessageSerializer serializer)
		{
			_serializer = serializer;
			Native = native;

			Native.ModelShutdown += (model, reason) =>
				{
					_isClosed = true;
				};

			_serializer = serializer;
		}

		public void Dispose()
		{
			if (Native != null)
			{
				if (Native.IsOpen)
					Native.Close();

				Native.Dispose();
			}
		}

		private void SafeNativeInvoke(Action<IModel> invokeAction)
		{
			try
			{
				if (!_isClosed)
					invokeAction(Native);
			}
			catch (AlreadyClosedException ex)
			{
				Failed(this, new ErrorEventArgs(ex));
			}
			catch (Exception ex)
			{
			}
		}

		public T UnpackAs<T>(IncomingDelivery delivery) where T : class
		{
			return _serializer.Deserialize<T>(delivery.Args.Body);
		}

		public void Reply<T>(T message, IncomingDelivery delivery)
		{
			var replyTo = delivery.Args.BasicProperties.ReplyToAddress;
			var correlationId = delivery.Args.BasicProperties.CorrelationId;
			if (replyTo == null || string.IsNullOrWhiteSpace(correlationId))
			{
				throw new InvalidOperationException("No ReplyToAddress or CorrelationId were found in delivery.");
			}

			Action<IBasicProperties> propsVisitor = props => { props.CorrelationId = correlationId; };

			Publish(new RabbitRoute(replyTo.ExchangeName, replyTo.RoutingKey), message, propsVisitor);
		}

		public void Accept(IncomingDelivery delivery)
		{
			SafeNativeInvoke(n => n.BasicAck(delivery.Args.DeliveryTag, false));
		}

		public void Reject(IncomingDelivery delivery, bool requeue)
		{
			SafeNativeInvoke(n => n.BasicNack(delivery.Args.DeliveryTag, false, requeue));
		}

		public void Publish<T>(RabbitRoute route, T message, Action<IBasicProperties> propsVisitor = null)
		{
			var props = Native.CreateBasicProperties();
			var serializedMessage = _serializer.Serialize(message);

			if (props.Headers == null)
				props.Headers = new Dictionary<string, object>();

			props.ContentType = _serializer.ContentType;
			//props.Timestamp = new AmqpTimestamp(DateTime.UtcNow.ToUnixTimestamp());

			//Bus.Configuration.MessageLabelHandler.Inject(props, message.Label);

			if (propsVisitor != null)
			{
				propsVisitor(props);
			}

			SafeNativeInvoke(n => n.BasicPublish(route.Exchange, route.RoutingKey, false, props, serializedMessage));
		}

		public string DeclareDefaultQueue()
		{
			var queueName = "";

			SafeNativeInvoke(n => queueName = n.QueueDeclare());

			return queueName;
		}

		public void Declare(Exchange exchange)
		{
			SafeNativeInvoke(n => n.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, new Dictionary<string, object>()));
		}

		public void Declare(Queue queue)
		{
			SafeNativeInvoke(n => n.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, new Dictionary<string, object>()));
		}

		public void Bind(Queue queue, Exchange exchange, string routingKey)
		{
			SafeNativeInvoke(n => n.QueueBind(queue.Name, exchange.Name, routingKey));
		}

		public void EnablePublishConfirmation()
		{
			SafeNativeInvoke(n => n.ConfirmSelect());
		}

		public CancellableQueueingConsumer BuildCancellableConsumer(CancellationToken cancellationToken)
		{
			return new CancellableQueueingConsumer(Native, cancellationToken);
		}

		public string StartConsuming(Queue queue, bool requireAccept, IBasicConsumer consumer)
		{
			var consumerTag = "";

			SafeNativeInvoke(n => consumerTag = n.BasicConsume(queue.Address, !requireAccept, consumer));

			return consumerTag;
		}

		public void StopConsuming(string consumerTag)
		{
			SafeNativeInvoke(n => n.BasicCancel(consumerTag));
		}

		public bool TryStopConsuming(string consumerTag)
		{
			try
			{
				Native.BasicCancel(consumerTag);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public ulong GetNextSeqNo()
		{
			var seqNo = 0UL;
			SafeNativeInvoke(n => seqNo = n.NextPublishSeqNo);
			return seqNo;
		}

		public void OnConfirmation(ConfirmationHandler handleConfirmation)
		{
			SafeNativeInvoke(n =>
				{
					n.BasicAcks += (model, args) => handleConfirmation(true, args.DeliveryTag, args.Multiple);
					n.BasicNacks += (model, args) => handleConfirmation(false, args.DeliveryTag, args.Multiple);
				});
		}
	}
}