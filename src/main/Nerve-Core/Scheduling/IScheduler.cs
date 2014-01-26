namespace Kostassoid.Nerve.Core.Scheduling
{
	using System;
	using Retlang.Fibers;

	public interface IScheduler : IDisposable
	{
		IFiber Fiber { get; }
	}
}