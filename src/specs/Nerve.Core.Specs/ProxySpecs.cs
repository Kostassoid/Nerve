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
	using System.Threading.Tasks;
	using Core;
	using Processing.Operators;
	using Proxy;
	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ProxySpecs
	{
		public interface ILogic
		{
			void A();
			void B(int i);
			string C();
			Task<string> D();
		}

		[Subject(typeof(Cell), "Proxy")]
		[Tags("Unit")]
		public class when_invoking_wrapped_object_parameterless_method
		{
			private static ILogic _logic;
			private static bool _received;
			private static Cell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<Invocation>().ReactWith(_ => _received = true);
				_logic = _cell.ProxyOf<ILogic>();
			};

			Because of = () => _logic.A();

			It should_send_invocation_message = () => _received.ShouldBeTrue();
		}

		[Subject(typeof(Cell), "Proxy")]
		[Tags("Unit")]
		public class when_invoking_wrapped_object_method_without_return
		{
			private static ILogic _logic;
			private static Invocation _received;
			private static Cell _cell;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<Invocation>().ReactWith(s => _received = s.Payload);
				_logic = _cell.ProxyOf<ILogic>();
			};

			Because of = () => _logic.B(13);

			It should_send_invocation_message = () => _received.ShouldNotBeNull();

			It should_contain_invocation_args = () => _received.Params.Single().ShouldEqual(13);
		}

		[Subject(typeof(Cell), "Proxy")]
		[Tags("Unit")]
		public class when_invoking_wrapped_object_method_with_simple_return
		{
			private static ILogic _logic;
			private static Invocation _received;
			private static Cell _cell;
			static string _returned;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<Invocation>().ReactWith(s =>
				{
					_received = s.Payload;
					s.Return("xyz");
				});
				_logic = _cell.ProxyOf<ILogic>();
			};

			Because of = () => _returned = _logic.C();

			It should_send_invocation_message = () => _received.ShouldNotBeNull();

			It should_return_value = () => _returned.ShouldEqual("xyz");
		}

		[Subject(typeof(Cell), "Proxy")]
		[Tags("Unit")]
		public class when_invoking_wrapped_object_method_with_task_return
		{
			private static ILogic _logic;
			private static Invocation _received;
			private static Cell _cell;
			static Task<string> _returned;

			private Cleanup after = () => _cell.Dispose();

			private Establish context = () =>
			{
				_cell = new Cell();
				_cell.OnStream().Of<Invocation>().ReactWith(s =>
				{
					_received = s.Payload;
					s.Return("xyz");
				});
				_logic = _cell.ProxyOf<ILogic>();
			};

			Because of = () => _returned = _logic.D();

			It should_send_invocation_message = () => _received.ShouldNotBeNull();

			It should_return_value = () => _returned.Result.ShouldEqual("xyz");
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}