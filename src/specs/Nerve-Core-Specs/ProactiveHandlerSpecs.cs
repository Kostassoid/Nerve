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
    using Handling;
    using Linking;
	using Machine.Specifications;
	using Model;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ProactiveHandlerSpecs
	{
		[Subject(typeof(IReactiveHandler), "Basic")]
		[Tags("Unit")]
		public class when_consuming_with_pulling_handler
		{
			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().Handle().Reactively(_ => _received = true);
			};

			Cleanup after = () => _cell.Dispose();

			Because of = () => _cell.Fire(new Ping());

			It should_be_handled = () => _received.ShouldBeTrue();

			static ICell _cell;
			static bool _received;
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}