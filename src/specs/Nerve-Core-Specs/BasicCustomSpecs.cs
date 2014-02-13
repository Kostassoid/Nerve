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
	using Linking;
	using Machine.Specifications;
	using Model;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BasicCustomSpecs
	{
		[Subject(typeof(Cell), "Basic")]
		[Tags("Unit")]
		public class when_firing_a_signal_with_attached_handler
		{
			Establish context = () =>
			{
				_cell = new SpecialCell();
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_be_handled = () => _cell.Received.ShouldBeTrue();

			static SpecialCell _cell;
		}

		class SpecialCell : Cell
		{
			public bool Received { get; private set; }

			public SpecialCell()
			{
				OnStream().Of<Ping>().ReactWith(_ => Received = true);
				//On(Stream().Of<Ping>(), _ => Received = true);
			}
		}


	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}