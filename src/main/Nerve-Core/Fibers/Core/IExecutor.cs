namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;
	using System.Collections.Generic;

	/// <summary>
    /// Executes pending action(s).
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Executes all actions.
        /// </summary>
        /// <param name="toExecute"></param>
        void Execute(List<Action> toExecute);

        ///<summary>
        /// Executes a single action. 
        ///</summary>
        ///<param name="toExecute"></param>
        void Execute(Action toExecute);
    }
}