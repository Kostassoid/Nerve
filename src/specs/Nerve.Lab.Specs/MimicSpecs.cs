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

namespace Kostassoid.Nerve.Lab.Specs
{
	using System;
	using System.Threading.Tasks;
	using Core;
	using FakeItEasy;
	using Machine.Specifications;
	using Mimic;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class MimicSpecs
	{
		public interface ILogic
		{
			void A();
			void B(int i);
			string C();
			Task<string> D();
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit")]
		public class when_sending_to_wrapped_object_using_parameterless_method
		{
			private static ILogic _logic;
			private static Cell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_logic = A.Fake<ILogic>();
				_cell = new Cell();
				_cell.BindTo(_logic);
			};

			private Because of = () => _cell.Send(Invoke<ILogic>.Using(l => l.A()));

			private It should_invoke_method = () =>
				A.CallTo(() => _logic.A()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit")]
		public class when_sending_to_wrapped_object_using_single_const_parameter_method
		{
			private static ILogic _logic;
			private static Cell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_logic = A.Fake<ILogic>();
				_cell = new Cell();
				_cell.BindTo(_logic);
			};

			private Because of = () => _cell.Send(Invoke<ILogic>.Using(l => l.B(13)));

			private It should_invoke_method = () =>
				A.CallTo(() => _logic.B(13)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit")]
		public class when_sending_to_wrapped_object_using_expression_parameter
		{
			private static ILogic _logic;
			private static Cell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_logic = A.Fake<ILogic>();
				_cell = new Cell();
				_cell.BindTo(_logic);
			};

			private Because of = () => _cell.Send(Invoke<ILogic>.Using(l => l.B(new Random().Next(100))));

			private It should_invoke_method = () =>
				A.CallTo(() => _logic.B(A<int>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}