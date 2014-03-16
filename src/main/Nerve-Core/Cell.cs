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
	using System;
	using System.Collections.Generic;

	using Linking;

	using Scheduling;

	using Signal;

	using Tools;
	using Tools.CodeContracts;

	/// <summary>
	///   Base Class implementation.
	///   Relays all incoming signals through all links.
	/// </summary>
	public class Cell : SignalProcessor, ICell
	{
		#region Fields

		private readonly ISet<ISignalProcessor> _links = new HashSet<ISignalProcessor>();

		private readonly IScheduler _scheduler;

		#endregion

		#region Constructors and Destructors

		public Cell(string name, Func<IScheduler> schedulerFactory)
		{
			Requires.NotNull(schedulerFactory, "schedulerFactory");

			Name = name;
			_scheduler = schedulerFactory();
		}

		public Cell(string name)
			: this(name, () => new ImmediateScheduler())
		{
		}

		public Cell(Func<IScheduler> schedulerFactory)
			: this(null, schedulerFactory)
		{
		}

		public Cell()
			: this(null, () => new ImmediateScheduler())
		{
		}

		~Cell()
		{
			Dispose(false);
		}

		#endregion

		#region Public Events

		public event SignalExceptionHandler Failed;

		#endregion

		#region Public Properties

		public string Name { get; private set; }

		#endregion

		#region Public Methods and Operators

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Fire<T>(T body) where T : class
		{
			Requires.NotNull(body, "body");

			OnSignal(new Signal<T>(body, Stacktrace.Empty));
		}

		public override bool OnFailure(SignalException exception)
		{
			if (Failed == null)
			{
				return base.OnFailure(exception);
			}

			Failed(this, exception);
			return true;
		}

		protected override void Process(ISignal signal)
		{
			_scheduler.Schedule(() => Relay(signal));
		}

		public IDisposable Attach(IHandler handler)
		{
			return Attach(new SignalHandlerWrapper(handler));
		}

		public IDisposable Attach<T>(IHandlerOf<T> handler) where T : class
		{
			return Attach(new SignalHandlerWrapper<T>(handler));
		}

		public ILinkJunction OnStream()
		{
			return new Link(this).Root;
		}

		public override string ToString()
		{
			return string.Format("{0}[{1}]", GetType().Name, Name ?? "unnamed");
		}

		#endregion

		#region Explicit Interface Methods

		IDisposable ISignalSource.Attach(ISignalProcessor signalProcessor)
		{
			return Attach(signalProcessor);
		}

		#endregion

		#region Methods

		public IDisposable Attach(ISignalProcessor signalProcessor)
		{
			Requires.NotNull(signalProcessor, "signalProcessor");

			lock (_links)
			{
				_links.Add(signalProcessor);
			}

			return new DisposableAction(() => Detach(signalProcessor));
		}

		internal void Detach(ISignalProcessor signalProcessor)
		{
			Requires.NotNull(signalProcessor, "signalProcessor");

			lock (_links)
			{
				_links.Remove(signalProcessor);
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (_links != null)
			{
				lock (_links)
				{
					_links.Clear();
				}
			}

			if (_scheduler != null)
			{
				_scheduler.Dispose();
			}
		}

		protected void Relay(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			lock (_links)
			{
				_links.ForEach(l => l.OnSignal(signal.Clone()));
			}
		}

		#endregion
	}
}