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

namespace Kostassoid.Nerve.Core.Processing.Operators
{
	internal class RootOperator : AbstractOperator
	{
		#region Constructors and Destructors

		public RootOperator(ILink link)
			: base(link)
		{
		}

		#endregion

		#region Public Methods and Operators

		protected override void Process(ISignal signal)
		{
			Next.OnSignal(signal);
		}

		public override void OnSignal(ISignal signal)
		{
			if (Next == null) return;

			Process(signal);
		}

		#endregion
	}
}