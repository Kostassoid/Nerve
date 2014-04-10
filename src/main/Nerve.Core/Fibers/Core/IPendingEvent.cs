namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;

	/// <summary>
    /// A scheduled event.
    /// </summary>
    internal interface IPendingEvent : IDisposable
    {
        /// <summary>
        /// Time of expiration for this event
        /// </summary>
        long Expiration { get; }

        /// <summary>
        /// Execute this event and optionally schedule another execution.
        /// </summary>
        /// <returns></returns>
        IPendingEvent Execute(long currentTime);
    }
}
