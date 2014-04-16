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

namespace Kostassoid.Nerve.Core.Specs.Tools
{
	using System;
	using Core.Tools;
	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class MemoizedFuncSpecs
    {
		[Subject(typeof(MemoizedFunc))]
		[Tags("Unit")]
		public class when_using_memoized_function_without_params
		{
			It should_return_cached_value = () =>
				{
					var counter = 0;
					Func<int> func = () =>
						{
							counter++;
							return 13;
						};

					var memoizedFunc = func.AsMemoized();

					memoizedFunc().ShouldEqual(13);
					memoizedFunc().ShouldEqual(13);
					memoizedFunc().ShouldEqual(13);

					counter.ShouldEqual(1);
				};
		}

		[Subject(typeof(MemoizedFunc))]
		[Tags("Unit")]
		public class when_using_memoized_function_with_one_param
		{
			It should_return_cached_values =
				() =>
					{
						var counter = 0;
						Func<int, int> func = x =>
							{
								counter++;
								return x*x;
							};

						var memoizedFunc = func.AsMemoized();

						memoizedFunc(5).ShouldEqual(25);
						memoizedFunc(4).ShouldEqual(16);
						memoizedFunc(4).ShouldEqual(16);
						memoizedFunc(5).ShouldEqual(25);
						memoizedFunc(4).ShouldEqual(16);
						memoizedFunc(5).ShouldEqual(25);

						counter.ShouldEqual(2);
					};
		}

		[Subject(typeof(MemoizedFunc))]
		[Tags("Unit")]
		public class when_using_memoized_function_with_two_params
		{
			It should_return_cached_values = () =>
				{
					var counter = 0;
					Func<int, int, int> func = (x, y) =>
						{
							counter++;
							return x*y;
						};

					var memoizedFunc = func.AsMemoized();

					memoizedFunc(2, 3).ShouldEqual(6);
					memoizedFunc(2, 5).ShouldEqual(10);
					memoizedFunc(4, 3).ShouldEqual(12);
					memoizedFunc(2, 3).ShouldEqual(6);
					memoizedFunc(2, 5).ShouldEqual(10);
					memoizedFunc(4, 3).ShouldEqual(12);

					counter.ShouldEqual(3);
				};
		}

		[Subject(typeof(MemoizedFunc))]
		[Tags("Unit")]
		public class when_using_memoized_function_with_three_params
		{
			It should_return_cached_values = () =>
				{
					var counter = 0;
					Func<int, int, int, int> func = (x, y, z) =>
						{
							counter++;
							return x*y*z;
						};

					var memoizedFunc = func.AsMemoized();

					memoizedFunc(2, 3, 4).ShouldEqual(24);
					memoizedFunc(2, 3, 5).ShouldEqual(30);
					memoizedFunc(2, 4, 3).ShouldEqual(24);
					memoizedFunc(2, 3, 5).ShouldEqual(30);
					memoizedFunc(2, 3, 4).ShouldEqual(24);
					memoizedFunc(2, 4, 3).ShouldEqual(24);

					counter.ShouldEqual(3);
				};
		}
    }
    // ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}
