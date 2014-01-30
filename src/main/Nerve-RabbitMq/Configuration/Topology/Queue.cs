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

namespace Kostassoid.Nerve.RabbitMq.Configuration.Topology
{
	public class Queue
	{
		public static QueueBuilder Named(string name)
		{
			return new QueueBuilder(name);
		}

		public string Name { get; internal set; }
		public bool Durable { get; internal set; }
		public bool Exclusive { get; internal set; }
		public bool AutoDelete { get; internal set; }

		public string Address
		{
			get { return Name; }
		}

		internal Queue(string name)
		{
			Name = name;
			Durable = false;
			Exclusive = false;
			AutoDelete = false;
		}

		protected bool Equals(Queue other)
		{
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Queue) obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{0}", Name);
		}
	}
}