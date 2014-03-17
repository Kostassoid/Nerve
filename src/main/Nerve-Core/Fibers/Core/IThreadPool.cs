namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System.Threading;

	/// <summary>
    /// A thread pool for executing asynchronous actions.
    /// </summary>
    public interface IThreadPool
    {
        /// <summary>
        /// Enqueue action for execution.
        /// </summary>
        /// <param name="callback"></param>
        void Queue(WaitCallback callback);
    }
}