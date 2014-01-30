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

namespace Kostassoid.Nerve.RabbitMq.Specs.Plumbing
{
	public class BackingQueueStatus
	{
		public int q1 { get; set; }
		public int q2 { get; set; }
		public List<object> delta { get; set; }
		public int q3 { get; set; }
		public int q4 { get; set; }
		public int len { get; set; }
		public int pending_acks { get; set; }
		public string target_ram_count { get; set; }
		public int ram_msg_count { get; set; }
		public int ram_ack_count { get; set; }
		public int next_seq_id { get; set; }
		public int PersistentCount { get; set; }
		public double avg_ingress_rate { get; set; }
		public double avg_egress_rate { get; set; }
		public double avg_ack_ingress_rate { get; set; }
		public double avg_ack_egress_rate { get; set; }
	}
}