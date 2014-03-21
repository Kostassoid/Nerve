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

namespace Kostassoid.Nerve.Core.Specs
{
	using Machine.Specifications;

	using Model;

	using Processing.Operators;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicCustomSpecs
	{
		private class SpecialCell : Cell
		{
			public SpecialCell()
			{
				OnStream().Of<Ping>().ReactWith(_ => Received = true);
			}

			public bool Received { get; private set; }
		}

		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_attached_handler
		{
			private static SpecialCell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () => { _cell = new SpecialCell(); };

			private Because of = () => _cell.Send(new Ping());

			private It should_be_handled = () => _cell.Received.ShouldBeTrue();
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}