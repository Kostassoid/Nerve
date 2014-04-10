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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Core;
	using FakeItEasy;
	using Machine.Specifications;
	using Mimic;
	using Core.Tools;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	#pragma warning disable 0169
	public class BindedPerformanceSpecs
	{
		public interface ILogic
		{
			void Foo(string p);
			string Boo(string p);
			Task<string> BooAsync(string p);
		}

		[Behaviors]
		public class fast_message_broker
		{
			protected static int SignalsCount;

			protected static ICell Cell;
			protected static ILogic Logic;
			protected static Action InvokingAction;

			private It should_be_faster_than_0_1_million_ops = () =>
			{
				var countdown = new CountdownEvent(SignalsCount);

				A.CallTo(() => Logic.Boo(A<string>.Ignored)).Invokes(() => countdown.Signal());

				var stopwatch = Stopwatch.StartNew();
				Enumerable.Range(0, SignalsCount).ForEach(_ => InvokingAction());

				countdown.Wait();
				stopwatch.Stop();

				var ops = SignalsCount * 1000L / stopwatch.ElapsedMilliseconds;
				Console.WriteLine("Ops / second: {0}", ops);
				ops.ShouldBeGreaterThan(100000);
			};
		}

		[Subject(typeof(Cell), "Mimic")]
		[Tags("Unit", "Unstable")]
		public class when_invoking_using_binded_object_method
		{
			protected static int SignalsCount = 100000;

			protected static ICell Cell;
			protected static ILogic Logic;
			protected static Action InvokingAction;

			Cleanup after = () => Cell.Dispose();

			Establish context = () =>
			{
				Logic = A.Fake<ILogic>();

				Cell = new Cell();
				Cell.BindTo(Logic);

				InvokingAction = () => Cell.Send(Invoke<ILogic>.Using(l => l.Boo("this")));
			};

			Behaves_like<fast_message_broker> _;
		}


	}

	#pragma warning restore 0169
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}