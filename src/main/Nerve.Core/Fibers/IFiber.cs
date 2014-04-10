namespace Kostassoid.Nerve.Core.Fibers
{
	using System;

	using Core;

	/// <summary>
    /// Enqueues pending actions for the context of execution (thread, pool of threads, message pump, etc.)
    /// </summary>
	internal interface IFiber : ISubscriptionRegistry, IExecutionContext, IScheduler, IDisposable
    {
        /// <summary>
        /// Start consuming actions.
        /// </summary>
        void Start();
    }
}
