﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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

namespace Kostassoid.Nerve.Core.Processing
{
	using System;

	using Tools;

	/// <summary>
	/// Base signal processor code.
	/// </summary>
	public abstract class Processor : IProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Signal processing entry point.
		/// </summary>
		/// <param name="signal"></param>
		public virtual void OnSignal(ISignal signal)
		{
			try
			{
				signal.Trace(this);
				Process(signal);
			}
			catch (Exception ex)
			{
				signal.MarkAsFaulted(ex);
			}
		}

		/// <summary>
		/// Processing failure handler.
		/// </summary>
		/// <param name="exception">Wrapped exception.</param>
		/// <returns>True if exception was handled.</returns>
		public virtual bool OnFailure(SignalException exception)
		{
			return false;
		}

		/// <summary>
		/// Processor logic.
		/// </summary>
		/// <param name="signal"></param>
		protected abstract void Process(ISignal signal);

		/// <summary>
		/// Builds processor readable description.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Processor[{0}]", GetType().BuildDescription());
		}

		#endregion

		/// <summary>
		/// Stub processor.
		/// </summary>
		public static IProcessor Stub { get { return StubProcessor.Instance; } }

		private class StubProcessor : IProcessor
		{
			// ReSharper disable once InconsistentNaming
			private static readonly Lazy<StubProcessor> _instance = new Lazy<StubProcessor>(() => new StubProcessor());
			public static StubProcessor Instance { get { return _instance.Value; } }

			private StubProcessor() { }

			public void OnSignal(ISignal signal)
			{
				// no-op
			}

			public bool OnFailure(SignalException exception)
			{
				return false;
			}
		}
	}

}