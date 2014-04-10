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
	using Helpers;
	using Machine.Specifications;
	using Core.Tools.CodeContracts;

	// ReSharper disable UnusedMember.Local
	// ReSharper disable InconsistentNaming
    public class CodeContractsSpecs
    {

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_requirement_on_null_value
		{
			It should_throw_argument_null_exception =
				() =>
				{
					string value = null;

					// ReSharper disable ExpressionIsAlwaysNull
					Action action = () => Requires.NotNull(value, "value");
					action.ShouldThrow<ArgumentNullException>();
					// ReSharper restore ExpressionIsAlwaysNull
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_requirement_on_non_null_value
		{
			It should_not_throw =
				() =>
				{
					var value = String.Empty;
					
					Action action = () => Requires.NotNull(value, "value");
					action.ShouldNotThrow<ArgumentNullException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_empty_requirement_on_null_value
		{
			It should_throw_argument_null_exception =
				() =>
				{
					string value = null;

					// ReSharper disable ExpressionIsAlwaysNull
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					action.ShouldThrow<ArgumentNullException>();
					// ReSharper restore ExpressionIsAlwaysNull
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_empty_requirement_on_empty_value
		{
			It should_throw_argument_exception =
				() =>
				{
					var value = String.Empty;
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					action.ShouldThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_empty_requirement_on_non_null_value
		{
			It should_not_throw =
				() =>
				{
					const string value = "zzz";
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					action.ShouldNotThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_true_requirement
		{
			It should_throw_if_false =
				() =>
				{
					Action action = () => Requires.True(true, "value");
					action.ShouldNotThrow<ArgumentException>();

					action = () => Requires.True(false, "value");
					action.ShouldThrow<ArgumentException>();

					action = () => Requires.True(false, "value", "oops");
					action.ShouldThrow<ArgumentException>().WithMessage("oops\r\nParameter name: value");

					action = () => Requires.True(false, "value", "simon says: {0}", "oops");
					action.ShouldThrow<ArgumentException>().WithMessage("simon says: oops\r\nParameter name: value");
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_not_null_or_empty_requirement_on_non_empty_array
		{
			It should_not_throw =
				() =>
				{
					var value = new[] {"boo", "foo"};
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					action.ShouldNotThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_not_null_or_empty_requirement_on_empty_array
		{
			It should_throw =
				() =>
				{
					var value = new string[0];
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					action.ShouldThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_not_null_or_empty_requirement_on_null_array
		{
			It should_throw =
				() =>
				{
					string[] value = null;
					// ReSharper disable ExpressionIsAlwaysNull
					Action action = () => Requires.NotNullOrEmpty(value, "value");
					// ReSharper restore ExpressionIsAlwaysNull
					action.ShouldThrow<ArgumentNullException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_without_null_elements_on_non_empty_array_without_nulls
		{
			It should_not_throw =
				() =>
				{
					var value = new[] {"boo", "foo"};
					Action action = () => Requires.NullOrWithNoNullElements(value, "value");
					action.ShouldNotThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_without_null_elements_on_null_array
		{
			It should_not_throw =
				() =>
				{
					var value = new string[0];
					Action action = () => Requires.NullOrWithNoNullElements(value, "value");
					action.ShouldNotThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_null_or_without_null_elements_on_non_empty_array_with_nulls
		{
			It should_not_throw =
				() =>
				{
					var value = new[] {"boo", null, "foo"};
					Action action = () => Requires.NullOrWithNoNullElements(value, "value");
					action.ShouldThrow<ArgumentException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_true_assumption_on_false_value
		{
			It should_throw_internal_exception =
				() =>
				{
					Action action = () => Assumes.True(false, "oops");
					action.ShouldThrow<Assumes.InternalErrorException>();

					action = () => Assumes.True(false, "simon says: {0}", "oops");
					action.ShouldThrow<Assumes.InternalErrorException>().WithMessage("simon says: oops");
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_checking_true_assumption_on_true_value
		{
			It should_not_throw =
				() =>
				{
					Action action = () => Assumes.True(true, "oops");
					action.ShouldNotThrow<Assumes.InternalErrorException>();
				};
		}

		[Subject("Tools")]
		[Tags("Unit")]
		public class when_failing_assumption
		{
			It should_throw_internal_exception =
				() =>
				{
					Action action = () => Assumes.Fail("oops");
					action.ShouldThrow<Assumes.InternalErrorException>().WithMessage("oops");

					action = () => Assumes.Fail();
					action.ShouldThrow<Assumes.InternalErrorException>();
				};
		}
    }
    // ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}
