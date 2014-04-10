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

namespace Kostassoid.Nerve.Core.Specs.Helpers
{
	using System;
	using Machine.Specifications;

	internal static class ActionEx
	{
		public static Exception ShouldThrow<T>(this Action action)
		{
			var ex = Catch.Exception(action);
			ex.ShouldBeOfExactType<T>();
			return ex;
		}

		public static Exception ShouldThrow(this Action action)
		{
			var ex = Catch.Exception(action);
			ex.ShouldNotBeNull();
			return ex;
		}

		public static void ShouldNotThrow<T>(this Action action)
		{
			var ex = Catch.Exception(action);
			if (ex == null) return;

			ex.ShouldNotBeOfExactType<T>();
		}

		public static void ShouldNotThrow(this Action action)
		{
			var ex = Catch.Exception(action);
			ex.ShouldBeNull();
		}
	}
}