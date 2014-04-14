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
	using System;
	using System.Threading.Tasks;
	using Machine.Specifications;

	using Model;

	using Processing.Operators;
	using Tpl;

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
	public class TplSpecs
	{
		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit")]
		[Ignore("Not ready")]
		public class when_requesting_value_using_task
		{
			static Cell _cell;

			static Task<string> _task;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return("pinged!"));
			};

			Because of = () => { _task = _cell.SendFor<string>(new Ping()); };

			It should_return_task = () => _task.ShouldNotBeNull();

			It should_have_value = () => _task.Result.ShouldEqual("pinged!");
		}

		[Subject(typeof(Cell), "TPL")]
		[Tags("Unit")]
		[Ignore("Not ready")]
		public class when_using_invalid_response_type
		{
			static Cell _cell;

			static Task<string> _task;
			static Exception _exception;

			Cleanup after = () => _cell.Dispose();

			Establish context = () =>
			{
				_cell = new Cell();

				_cell.OnStream().Of<Ping>().ReactWith(s => s.Return(new Pong()));
			};

			Because of = () => _exception = Catch.Exception(() =>
				{
					_task = _cell.SendFor<string>(new Ping());
					_task.Wait(1000);
				});

			It should_complete_the_task = () => _task.IsCompleted.ShouldBeTrue();

			It should_throw = () => _exception.ShouldBeOfExactType<ArgumentException>();

			It should_mark_task_as_failed = () => _task.IsFaulted.ShouldBeTrue();
		}

	}

	// ReSharper restore UnusedMember.Local
	// ReSharper restore InconsistentNaming
}