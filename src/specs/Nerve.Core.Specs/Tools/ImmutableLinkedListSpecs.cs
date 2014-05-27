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
	using System.Linq;
	using Core.Tools.Collections;
	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ImmutableLinkedListSpecs
	{
		[Subject(typeof(ImmutableLinkedList<>))]
		[Tags("Unit")]
		public class empty_list
		{
			static IImmutableLinkedList<int> _list;

			Because of = () =>
			{
				_list = ImmutableLinkedList<int>.Empty;
			};

			It should_be_empty = () => _list.IsEmpty.ShouldBeTrue();

			It should_have_0_items = () => _list.Count.ShouldEqual(0);
		}

		[Subject(typeof(ImmutableLinkedList<>))]
		[Tags("Unit")]
		public class when_prepending_empty_list
		{
			static IImmutableLinkedList<int> _original;
			static IImmutableLinkedList<int> _target;

			Establish context = () =>
			{
				_original = ImmutableLinkedList<int>.Empty;
			};

			Because of = () =>
			{
				_target = _original.Prepend(13);
			};

			It should_not_alter_original_ = () => _original.IsEmpty.ShouldBeTrue();

			It should_not_be_empty = () => _target.IsEmpty.ShouldBeFalse();

			It should_have_1_item = () => _target.Count.ShouldEqual(1);

			It should_have_added_item = () => _target.Single().ShouldEqual(13);
		}

		[Subject(typeof(ImmutableLinkedList<>))]
		[Tags("Unit")]
		public class when_prepending_non_empty_list
		{
			static IImmutableLinkedList<int> _original;
			static IImmutableLinkedList<int> _target;

			Establish context = () =>
			{
				_original = ImmutableLinkedList<int>.Empty.Prepend(13);
			};

			Because of = () =>
			{
				_target = _original.Prepend(666);
			};

			It should_not_alter_original_ = () => _original.Count.ShouldEqual(1);

			It should_not_be_empty = () => _target.IsEmpty.ShouldBeFalse();

			It should_have_correct_count = () => _target.Count.ShouldEqual(2);

			It should_have_prepended_item = () => _target.First().ShouldEqual(666);
		}
	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}