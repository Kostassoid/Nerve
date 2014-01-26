namespace Kostassoid.Nerve.Core
{
	using System;
	using Pipeline;
	using Retlang.Channels;
	using Retlang.Fibers;
	using Scheduling;
	using Signal;

	public class Link : IDisposable
	{
		IFiber _fiber;
		IChannel<ISignal> _channel;

		public IScheduler Scheduler { get; internal set; }

		public IAgent Owner { get; private set; }
		public IPipelineStep PipelineStep { get; private set; }

		public Link(IAgent owner)
		{
			PipelineStep = new PipelineStep<object>(this);
			Scheduler = new ImmediateScheduler();
			Owner = owner;
		}

		public void Process(ISignal signal)
		{
			_channel.Publish(signal);
		}

		public void Subscribe()
		{
			_fiber = Scheduler.Fiber;
			_fiber.Start();
			_channel = new Channel<ISignal>();
			_channel.Subscribe(_fiber, PipelineStep.Execute);

			Owner.Subscribe(this);
		}

		public void Dispose()
		{
			if (_channel != null)
				_channel.ClearSubscribers();

			if (Scheduler != null)
				Scheduler.Dispose();
		}
	}
}