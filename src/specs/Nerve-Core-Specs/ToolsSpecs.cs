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
	using System.Collections;
	using System.Collections.Generic;

	using Machine.Specifications;

	using Processing.Operators;

	using Tools;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ToolsSpecs
	{
		[Subject(typeof(TypeEx))]
		[Tags("Unit")]
		public class when_building_type_description
		{
			It should_handle_simple_types = () =>
				{
					typeof(int).BuildDescription()
						.ShouldEqual("Int32");

					typeof(string).BuildDescription()
						.ShouldEqual("String");

					typeof(object).BuildDescription()
						.ShouldEqual("Object");

					typeof(Cell).BuildDescription()
						.ShouldEqual("Cell");
				};

			It should_handle_generics = () =>
				{
					typeof(IList<int>).BuildDescription()
						.ShouldEqual("IList[Int32]");

					typeof(IDictionary<string, object>).BuildDescription()
						.ShouldEqual("IDictionary[String,Object]");
				};

			It should_handle_nested = () =>
				{
					typeof(IList<IList<IList<int>>>).BuildDescription()
						.ShouldEqual("IList[IList[IList[Int32]]]");

					typeof(Dictionary<String, List<Func<String, Boolean>>>).BuildDescription()
						.ShouldEqual("Dictionary[String,List[Func[String,Boolean]]]");
				};

			It should_handle_operators = () =>
				{
					typeof(GateOp.GateOperator).BuildDescription()
						.ShouldEqual("Gate");

					typeof(OfOp.OfOperator<object>).BuildDescription()
						.ShouldEqual("Of[Object]");

					typeof(IList<MapOp.MapOperator<IList<int>, IList<string>>>).BuildDescription()
						.ShouldEqual("IList[Map[IList[Int32],IList[String]]]");
				};
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}