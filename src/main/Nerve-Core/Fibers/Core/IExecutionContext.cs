namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;

	/// <summary>
    /// Context of execution.
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Enqueue a single action.
        /// </summary>
        /// <param name="action"></param>
        void Enqueue(Action action);
    }
}