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

using System.Text;
using Newtonsoft.Json;

namespace Kostassoid.Nerve.RabbitMq.Serialization
{
	public class JsonNetSerializer : IMessageSerializer
	{
		public string ContentType
		{
			get { return "application/json"; }
		}

		public byte[] Serialize<T>(T message)
		{
			var json = JsonConvert.SerializeObject(message);

			return Encoding.UTF8.GetBytes(json);
		}

		public T Deserialize<T>(byte[] message)
		{
			var decoded = Encoding.UTF8.GetString(message);

			return JsonConvert.DeserializeObject<T>(decoded);
		}
	}
}