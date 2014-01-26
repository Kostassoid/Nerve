namespace Kostassoid.Nerve.Core.Tools
{
	using System;

	public class DisposableAction : IDisposable
	{
		readonly Action _disposeAction;
		bool _isDisposed;

		public DisposableAction(Action disposeAction)
		{
			_disposeAction = disposeAction;
		}

		public void Dispose()
		{
			if (_isDisposed) return;
			_disposeAction();
			_isDisposed = true;
		}
	}
}