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
	public class Cell : ICell
	{
		#region Fields

		private readonly ISet<IHandler> _links = new HashSet<IHandler>();

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

		public event SignalExceptionHandler Failed = (cell, exception) => { };

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

			OnSignal(new Signal<T>(body, StackTrace.Empty));
		}

		public void OnSignal(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			signal.Trace(this);

			_scheduler.Schedule(() => Relay(signal));
		}

		public virtual bool OnFailure(SignalException exception)
		{
			Failed(this, exception);

			return true;
		}

		public IDisposable Attach<T>(IHandlerOf<T> handler) where T : class
		{
			return Attach(new HandlerConnector<T>(handler) as IHandler);
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

		IDisposable ISignalSource.Attach(IHandler handler)
		{
			return Attach(handler);
		}

		#endregion

		#region Methods

		internal IDisposable Attach(IHandler handler)
		{
			Requires.NotNull(handler, "handler");

			lock (_links)
			{
				_links.Add(handler);
			}

			return new DisposableAction(() => Detach(handler));
		}

		internal void Detach(IHandler handler)
		{
			Requires.NotNull(handler, "handler");

			lock (_links)
			{
				_links.Remove(handler);
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

			_links.ForEach(l => l.OnSignal(signal));
		}

		#endregion
	}
}