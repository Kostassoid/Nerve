namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;

	/// <summary>
    /// Methods for scheduling actions that will be executed in the future.
    /// </summary>
    internal interface IScheduler
    {
        /// <summary>
        /// Schedules an action to be executed once.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        IDisposable Schedule(Action action, long firstInMs);

        /// <summary>
        /// Schedule an action to be executed on a recurring interval.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <param name="regularInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs);
    }
}
