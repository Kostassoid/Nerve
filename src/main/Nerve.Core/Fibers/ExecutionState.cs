namespace Kostassoid.Nerve.Core.Fibers
{
    ///<summary>
    /// Fiber execution state management
    ///</summary>
    internal enum ExecutionState
    {
        ///<summary>
        /// Created but not running
        ///</summary>
        Created,
        ///<summary>
        /// After start
        ///</summary>
        Running,
        ///<summary>
        /// After stopped
        ///</summary>
        Stopped
    }
}
