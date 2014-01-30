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
using System.Linq;
using System.Net;
using RestSharp;

namespace Kostassoid.Nerve.RabbitMq.Specs.Plumbing
{
	public class Broker
	{
		private readonly string connection;

		public Broker(string connection)
		{
			this.connection = connection;
		}

		internal void DeleteHost(string vhostName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/vhosts/{name}", Method.DELETE);
			request.AddUrlSegment("name", vhostName);
			client.Execute(request);
		}

		internal void CreateHost(string vhostName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/vhosts/{name}", Method.PUT);
			request.AddUrlSegment("name", vhostName);
			client.Execute(request);
		}

		internal void CreateUser(string vhostName, string user)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/permissions/{name}/{user}", Method.PUT);
			request.AddUrlSegment("name", vhostName);
			request.AddUrlSegment("user", user);
			request.AddBody(new {Configure = ".*", Write = ".*", Read = ".*"});
			client.Execute(request);
		}

		public bool HasExchange(string exchangeName, string vhostName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/exchanges/{name}", Method.GET);
			request.AddUrlSegment("name", vhostName);
			var response = client.Execute<List<Exchange>>(request);

			return response.StatusCode == HttpStatusCode.OK
				   && response.Data.Any(ex => ex.Name == exchangeName);
		}

		public bool HasQueue(string queueName, string vhostName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/queues/{name}", Method.GET);
			request.AddUrlSegment("name", vhostName);
			var response = client.Execute<List<Queue>>(request);

			return response.StatusCode == HttpStatusCode.OK
				   && response.Data.Any(queue => queue.Name == queueName);
		}

		internal void CreateQueue(string vhostName, string queueName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/queues/{vhost}/{queue}", Method.PUT);
			request.AddUrlSegment("vhost", vhostName);
			request.AddUrlSegment("queue", queueName);
			request.AddHeader("Accept", string.Empty);
			client.Execute(request);
		}

		public void CreateBind(string vhostName, string exchangeName, string queueName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/bindings/{vhost}/e/{exchange}/q/{queue}", Method.POST);
			request.AddUrlSegment("vhost", vhostName);
			request.AddUrlSegment("exchange", exchangeName);
			request.AddUrlSegment("queue", queueName);
			client.Execute(request);
		}

		internal List<Message> GetMessages(string vhostName, string queueName)
		{
			var options = new
			{
				vhost = vhostName,
				name = queueName,
				count = "1",
				requeue = "true",
				encoding = "auto",
				truncate = "50000"
			};
			var client = CreateClient();
			var request = CreateRequest("/api/queues/{vhost}/{queue}/get", Method.POST);
			request.AddUrlSegment("vhost", vhostName);
			request.AddUrlSegment("queue", queueName);
			request.AddBody(options);
			var response = client.Execute<List<Message>>(request);
			return response.StatusCode == HttpStatusCode.OK ? response.Data : new List<Message>();
		}

		public IList<Queue> GetQueues(string vhostName)
		{
			var client = CreateClient();
			var request = CreateRequest("/api/queues/{name}", Method.GET);
			request.AddUrlSegment("name", vhostName);
			var response = client.Execute<List<Queue>>(request);

			return response.StatusCode == HttpStatusCode.OK ? response.Data : new List<Queue>();
		}

		private IRestClient CreateClient()
		{
			var client = new RestClient(connection)
			{
				Authenticator = new HttpBasicAuthenticator("guest", "guest")
			};
			client.AddDefaultHeader("Content-Type", "application/json; charset=utf-8");
			return client;
		}

		private static RestRequest CreateRequest(string resource, Method method)
		{
			var request = new RestRequest(resource, method)
			{
				JsonSerializer = new RabbitJsonSerializer(),
				RequestFormat = DataFormat.Json
			};
			request.AddHeader("Accept", string.Empty);
			return request;
		}
	}
}