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

using Kostassoid.Nerve.Core;
using Kostassoid.Nerve.Core.Pipeline;
using Kostassoid.Nerve.RabbitMq.Configuration;
using Kostassoid.Nerve.RabbitMq.Configuration.Topology;
using Kostassoid.Nerve.RabbitMq.Serialization;
using Kostassoid.Nerve.RabbitMq.Specs.Model;
using Machine.Specifications;

namespace Kostassoid.Nerve.RabbitMq.Specs
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicSpecs
	{
		[Subject(typeof(RabbitEndpoint), "Basic")]
		[Tags("Integration")]
		[Ignore("Not ready")]
		public class when_firing_a_signal_using_two_rabbit_endpoints
		{
			Establish context = () =>
			{
				_senderEndpoint = new RabbitEndpoint("Sender");
				_senderEndpoint.Start(cfg =>
				{
					cfg.ConnectTo("amqp://localhost/integration");
					cfg.SetTypeHandler(new DefaultTypeHandler());
					cfg.UseSerializer(new JsonNetSerializer());
				});

				_sender = _senderEndpoint.BuildCell(cfg =>
				{
					cfg.RouteTo(cfg.Declare(Exchange.Named("msg.num")));
				});

				_receiverEndpoint = new RabbitEndpoint("Receiver");
				_receiverEndpoint.Start(cfg =>
				{
					cfg.ConnectTo("amqp://localhost/integration");
					cfg.SetTypeHandler(new DefaultTypeHandler());
					cfg.UseSerializer(new JsonNetSerializer());
				});

				_receiver = _receiverEndpoint.BuildCell(cfg =>
				{
					var e = cfg.Declare(Exchange.Named("msg.num"));
					var q = cfg.Declare(Queue.Named("msg.num"));
					cfg.Bind(e, q);
					cfg.Listen(q);
				});

				_receiver.OnStream().Of<Num>().ReactWith(_ => _received = true);
			};

			Cleanup after = () =>
			{
				_sender.Dispose();
				_receiver.Dispose();
				_senderEndpoint.Dispose();
				_receiverEndpoint.Dispose();
			};

			Because of = () => _sender.Fire(new Num(13));

			It should_be_handled = () => _received.ShouldBeTrue();

			static RabbitEndpoint _senderEndpoint;
			static RabbitEndpoint _receiverEndpoint;
			static ICell _sender;
			static ICell _receiver;
			static bool _received;
		}


	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}