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

using RabbitMQ.Client;

namespace Kostassoid.Nerve.RabbitMq.Configuration.Topology
{
	public class ExchangeBuilder
	{
		internal Exchange Instance { get; private set; }

		internal ExchangeBuilder(string name)
		{
			Instance = new Exchange(name);
		}

		public ExchangeBuilder OfType(string type)
		{
			Instance.Type = type;
			return this;
		}

		public ExchangeBuilder Direct
		{
			get
			{
				Instance.Type = ExchangeType.Direct;
				return this;
			}
		}

		public ExchangeBuilder Fanout
		{
			get
			{
				Instance.Type = ExchangeType.Fanout;
				return this;
			}
		}

		public ExchangeBuilder Topic
		{
			get
			{
				Instance.Type = ExchangeType.Topic;
				return this;
			}
		}

		public ExchangeBuilder Durable
		{
			get
			{
				Instance.Durable = true;
				return this;
			}
		}

		public ExchangeBuilder AutoDelete
		{
			get
			{
				Instance.AutoDelete = true;
				return this;
			}
		}
	}
}