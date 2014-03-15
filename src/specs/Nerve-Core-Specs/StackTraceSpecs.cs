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
	using System.Linq;

	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class StackTraceSpecs
	{
		[Subject(typeof(Stacktrace))]
		[Tags("Unit")]
		public class when_creating_stacktrace_from_existing_cell
		{
			private static ICell _cell;

			private static Stacktrace _stack;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () => { _cell = new Cell(); };

			private Because of = () => _stack = new Stacktrace(_cell);

			private It should_have_one_item_in_stack = () => _stack.Frames.Count().ShouldEqual(1);

			private It should_have_root_set_to_original_cell = () => _stack.Frames.First().ShouldEqual(_cell);

			private It should_have_top_set_to_original_cell = () => _stack.Frames.Last().ShouldEqual(_cell);
		}

		[Subject(typeof(Stacktrace))]
		[Tags("Unit")]
		public class when_pushing_to_stacktrace
		{
			private static ICell _cell;

			private static Stacktrace _stack;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () => { _cell = new Cell(); };

			private Because of = () => _stack = new Stacktrace(_cell);

			private It should_have_one_item_in_stack = () => _stack.Frames.Count().ShouldEqual(1);

			private It should_have_root_set_to_original_cell = () => _stack.Frames.First().ShouldEqual(_cell);

			private It should_have_top_set_to_original_cell = () => _stack.Frames.Last().ShouldEqual(_cell);
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}