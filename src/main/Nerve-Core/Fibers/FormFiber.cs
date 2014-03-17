namespace Kostassoid.Nerve.Core.Fibers
{
	using System.ComponentModel;

	using Core;

	///<summary>
    /// Allows interaction with Windows Forms.  Transparently moves actions onto the Form's thread.
    ///</summary>
    internal class FormFiber : GuiFiber
    {
        /// <summary>
        /// Creates an instance.
        /// </summary>
        public FormFiber(ISynchronizeInvoke invoker, IExecutor executor)
            : base(new FormAdapter(invoker), executor)
        {
        }
    }
}