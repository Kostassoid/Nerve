namespace Kostassoid.Nerve.Core.Fibers
{
	using System;
	using System.Windows.Threading;

	using Core;

	internal class DispatcherAdapter : IExecutionContext
    {
        private readonly Dispatcher _dispatcher;
        private readonly DispatcherPriority _priority;

        public DispatcherAdapter(Dispatcher dispatcher, DispatcherPriority priority)
        {
            _dispatcher = dispatcher;
            _priority = priority;
        }

        public void Enqueue(Action action)
        {
            _dispatcher.BeginInvoke(action, _priority);
        }
    }
}