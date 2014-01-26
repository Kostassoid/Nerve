namespace Kostassoid.Nerve.Core.Pipeline
{
	using System;
	using Scheduling;
	using Signal;

	internal class PipelineStep<T> : IPipelineStep<T> where T : class
	{
		Action<ISignal<T>> _next;

		public Link Link { get; private set; }

		public PipelineStep(Link link)
		{
			Link = link;
		}

		public void Execute(ISignal<T> item)
		{
			if (_next == null) return;
			_next(item);
		}


		public void Attach(Action<ISignal<T>> action)
		{
			_next = action;
		}

		public void ScheduleOn(IScheduler scheduler)
		{
			Link.Scheduler = scheduler;
		}

		void IPipelineStep.Execute(ISignal signal)
		{
			Execute(signal.As<T>());
		}

		void IPipelineStep.Attach(Action<ISignal> action)
		{
			_next = action;
		}
	}
}