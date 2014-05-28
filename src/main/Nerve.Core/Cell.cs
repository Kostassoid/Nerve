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

	using Processing;

	using Scheduling;

	using Tools;
	using Tools.CodeContracts;

	/// <summary>
	/// Base Class implementation.
	/// Relays all incoming signals through all attached links.
	/// </summary>
	public class Cell : Processor, ICell
	{
		private readonly ISet<IProcessor> _links = new HashSet<IProcessor>();

		private readonly IScheduler _scheduler;

		private readonly object _sync = new object();

		/// <summary>
		/// Constructs new cell.
		/// </summary>
		/// <param name="name">Cell name.</param>
		/// <param name="schedulerFactory">Scheduler factory.</param>
		public Cell(string name, Func<IScheduler> schedulerFactory)
		{
			Requires.NotNull(schedulerFactory, "schedulerFactory");

			Name = name;
			_scheduler = schedulerFactory();
		}

		/// <summary>
		/// Constructs new cell using default immediate scheduler.
		/// </summary>
		/// <param name="name">Cell name.</param>
		public Cell(string name)
			: this(name, () => new ImmediateScheduler())
		{
		}

		/// <summary>
		/// Constructs new unnamed cell.
		/// </summary>
		/// <param name="schedulerFactory">Scheduler factory.</param>
		public Cell(Func<IScheduler> schedulerFactory)
			: this(null, schedulerFactory)
		{
		}

		/// <summary>
		/// Constructs new unnamed cell using default immediate scheduler.
		/// </summary>
		public Cell()
			: this(null, () => new ImmediateScheduler())
		{
		}

		/// <summary>
		/// Cell finalizer.
		/// </summary>
		~Cell()
		{
			Dispose(false);
		}

		/// <summary>
		/// Failure handling event.
		/// </summary>
		public event SignalExceptionHandler Failed;

		/// <summary>
		/// Optional cell name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Disposes cell freeing all underlying resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Sends signal.
		/// </summary>
		/// <param name="signal">Signal to send.</param>
		public void Send(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			OnSignal(signal);
		}

		/// <summary>
		/// Sends new signal using payload.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="payload">Payload body.</param>
		public void Send<T>(T payload)
		{
			//Requires.True(Equals(payload, default(T)), "payload");

			OnSignal(Signal.Of(payload));
		}

		/// <summary>
		/// Sends new signal using payload and explicit callback processor.
		/// </summary>
		/// <typeparam name="T">Payload type.</typeparam>
		/// <param name="payload">Payload body.</param>
		/// <param name="callback">Callback processor.</param>
		public void Send<T>(T payload, IProcessor callback)
		{
			//Requires.True(Equals(payload, default(T)), "payload");
			Requires.NotNull(callback, "callback");

			OnSignal(Signal.Of(payload, callback));
		}

		/// <summary>
		/// Handles signal processing exception.
		/// </summary>
		/// <param name="exception">Wrapped exception.</param>
		/// <returns>Handling result. True if any subscriber present.</returns>
		public override bool OnFailure(SignalException exception)
		{
			if (Failed == null)
			{
				return base.OnFailure(exception);
			}

			Failed(this, exception);
			return true;
		}

		/// <summary>
		/// Processes (schedules) incoming signal.
		/// </summary>
		/// <param name="signal">Signal to process.</param>
		protected override void Process(ISignal signal)
		{
			_scheduler.Enqueue(() => Relay(signal));
		}

		/// <summary>
		/// Attaching (subscribing) untyped consumer to this signal source.
		/// </summary>
		/// <param name="consumer">Consumer to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public IDisposable Attach(IConsumer consumer)
		{
			return Attach(new ConsumerWrapper(consumer));
		}

		/// <summary>
		/// Attaches (subscribes) typed consumer to stream event.
		/// </summary>
		/// <param name="consumer">Consumer to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public IDisposable Attach<T>(IConsumerOf<T> consumer)
		{
			return Attach(new ConsumerWrapper<T>(consumer));
		}

		/// <summary>
		/// Starts building signal stream processing chain.
		/// </summary>
		/// <returns>Link extending point.</returns>
		public ILinkJunction OnStream()
		{
			return new Link(this).Root;
		}

		/// <summary>
		/// Builds readable cell description.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}[{1}]", GetType().Name, Name ?? "unnamed");
		}

		/// <summary>
		/// Attaching (subscribing) processor to this signal source.
		/// </summary>
		/// <param name="processor">Processor to attach.</param>
		/// <returns>Unsubscribing disposable object.</returns>
		public IDisposable Attach(IProcessor processor)
		{
			Requires.NotNull(processor, "processor");

			lock (_sync)
			{
				_links.Add(processor);
			}

			return new DisposableAction(() => Detach(processor));
		}

		internal void Detach(IProcessor processor)
		{
			Requires.NotNull(processor, "processor");

			lock (_sync)
			{
				_links.Remove(processor);
			}
		}

		/// <summary>
		/// Performs object cleanup.
		/// </summary>
		/// <param name="isDisposing"></param>
		protected virtual void Dispose(bool isDisposing)
		{
			lock (_sync)
			{
				if (_links != null)
				{
					_links.Clear();
				}
			}

			if (_scheduler != null)
			{
				_scheduler.Dispose();
			}
		}

		private void Relay(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			lock (_sync)
			{
				_links.ForEach(l => l.OnSignal(signal.Clone()));
			}
		}
	}
}