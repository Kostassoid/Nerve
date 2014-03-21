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
	public class HeadersSpecs
	{
		[Subject(typeof(Headers))]
		[Tags("Unit")]
		public class when_getting_empty_headers
		{
			static Headers _headers;

			Because of = () => _headers = Headers.Empty;

			It should_have_no_elemets = () => _headers.Items.ShouldBeEmpty();

			It should_be_the_same_as_empty = () => _headers.ShouldBeTheSameAs(Headers.Empty);
		}

		[Subject(typeof(Headers))]
		[Tags("Unit")]
		public class when_adding_unique_keys_to_headers
		{
			static Headers _original;
			static Headers _new;

			Establish context = () => _original = Headers.Empty;

			Because of = () => _new = _original.With("a", "1").With("b", "2");

			It should_have_items_available = () =>
			{
				_new["a"].ShouldEqual("1");
				_new["b"].ShouldEqual("2");
			};

			It should_not_alter_original_object = () => _original.Items.ShouldBeEmpty();
		}

		[Subject(typeof(Headers))]
		[Tags("Unit")]
		public class when_adding_non_unique_keys_to_headers
		{
			static Headers _original;
			static Headers _new;

			Establish context = () => _original = Headers.Empty.With("a", "1").With("b", "2");

			Because of = () => _new = _original.With("a", "13");

			It should_not_have_duplicate_keys = () => _new.Items.Count().ShouldEqual(2);

			It should_have_items_available = () =>
			{
				_new["a"].ShouldEqual("13");
				_new["b"].ShouldEqual("2");
			};
		}

		[Subject(typeof(Headers))]
		[Tags("Unit")]
		public class when_removing_existing_item_from_headers
		{
			static Headers _original;
			static Headers _new;

			Establish context = () => _original = Headers.Empty.With("a", "1").With("b", "2").With("c", "3");

			Because of = () => _new = _original.Without("b");

			It should_not_have_removed_item = () =>
			{
				_new.Items.Count().ShouldEqual(2);
				
				_new["a"].ShouldEqual("1");
				_new["c"].ShouldEqual("3");
			};
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}