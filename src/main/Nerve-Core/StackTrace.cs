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

namespace Kostassoid.Nerve.Core
{
	using Processing;

	using Tools.Collections;

	/// <summary>
	/// Processing stack implementation.
	/// </summary>
	public class Stacktrace : IProcessingStack
	{
		// ReSharper disable InconsistentNaming
		static readonly Stacktrace _empty = new Stacktrace(ImmutableLinkedList<IProcessor>.Empty);
		// ReSharper restore InconsistentNaming

		readonly IImmutableLinkedList<IProcessor> _frames = ImmutableLinkedList<IProcessor>.Empty;
		
		/// <summary>
		/// Processors trace list.
		/// </summary>
		public IImmutableLinkedList<IProcessor> Frames {
			get
			{
				return _frames;
			}
		}

		/// <summary>
		/// Empty stacktrace.
		/// </summary>
		public static Stacktrace Empty
		{
			get
			{
				return _empty;
			}
		}

		private Stacktrace()
		{}

		private Stacktrace(IImmutableLinkedList<IProcessor> frames)
		{
			_frames = frames;
		}

		/// <summary>
		/// Returns new stacktrace with added processor record.
		/// </summary>
		/// <param name="processor"></param>
		/// <returns></returns>
		public Stacktrace With(IProcessor processor)
		{
			return new Stacktrace(_frames.Prepend(processor));
		}
	}
}