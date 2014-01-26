namespace Kostassoid.Nerve.Core.Pipeline
{
	using System;
	using Scheduling;
	using Signal;

	public interface IPipelineStep
	{
		Link Link { get; }

		void Execute(ISignal item);
		void Attach(Action<ISignal> action);
	}

	public interface IPipelineStep<T> : IPipelineStep where T : class
	{
		void Execute(ISignal<T> item);
		void Attach(Action<ISignal<T>> action);
		void ScheduleOn(IScheduler scheduler);
	}
}