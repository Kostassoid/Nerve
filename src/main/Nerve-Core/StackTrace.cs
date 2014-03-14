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

	public class Stacktrace
	{
		#region Fields

		private readonly IList<ISignalProcessor> _frames = new List<ISignalProcessor>();

		#endregion

		#region Constructors and Destructors

		public Stacktrace()
		{
		}

		public Stacktrace(ISignalProcessor root)
		{
			Requires.NotNull(root, "root");

			_frames.Add(root);
		}

		protected Stacktrace(IList<ISignalProcessor> frames)
		{
			Requires.NotNullOrEmpty(frames, "Frames");

			_frames = new List<ISignalProcessor>(frames);
		}

		#endregion

		#region Public Properties

		public IEnumerable<ISignalProcessor> Frames
		{
			get
			{
				return _frames;
			}
		}

		public ISignalProcessor Root
		{
			get
			{
				return _frames.FirstOrDefault();
			}
		}

		public ISignalProcessor Top
		{
			get
			{
				return _frames.LastOrDefault();
			}
		}

		public static Stacktrace Empty
		{
			get
			{
				return new Stacktrace();
			}
		}

		#endregion

		#region Public Methods and Operators

		public Stacktrace Clone()
		{
			return new Stacktrace(_frames);
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
			return Equals((Stacktrace)obj);
		}

		public override int GetHashCode()
		{
			return (_frames != null ? _frames.GetHashCode() : 0);
		}

		public void Trace(ISignalProcessor cell)
		{
			Requires.NotNull(cell, "cell");

			_frames.Add(cell);
		}

		#endregion

		#region Methods

		protected bool Equals(Stacktrace other)
		{
			return _frames.SequenceEqual(other._frames);
		}

		#endregion
	}
}