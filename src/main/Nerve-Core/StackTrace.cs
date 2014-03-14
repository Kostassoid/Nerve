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
	using System.Collections.Generic;
	using System.Linq;

	using Tools.CodeContracts;

	public class StackTrace
	{
		#region Fields

		private readonly IList<ICell> _frames = new List<ICell>();

		#endregion

		#region Constructors and Destructors

		public StackTrace(ICell root)
		{
			Requires.NotNull(root, "root");

			_frames.Add(root);
		}

		protected StackTrace(IList<ICell> frames)
		{
			Requires.NotNullOrEmpty(frames, "Frames");

			_frames = frames;
		}

		#endregion

		#region Public Properties

		public IEnumerable<ICell> Frames
		{
			get
			{
				return _frames;
			}
		}

		public ICell Root
		{
			get
			{
				return _frames.FirstOrDefault();
			}
		}

		public ICell Top
		{
			get
			{
				return _frames.LastOrDefault();
			}
		}

		#endregion

		#region Public Methods and Operators

		public StackTrace Clone()
		{
			return new StackTrace(_frames);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((StackTrace)obj);
		}

		public override int GetHashCode()
		{
			return (_frames != null ? _frames.GetHashCode() : 0);
		}

		public void Push(ICell cell)
		{
			Requires.NotNull(cell, "cell");

			_frames.Add(cell);
		}

		#endregion

		#region Methods

		protected bool Equals(StackTrace other)
		{
			return Equals(_frames, other._frames);
		}

		#endregion
	}
}