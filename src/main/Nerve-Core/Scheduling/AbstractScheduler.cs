namespace Kostassoid.Nerve.Core.Scheduling
{
	using Retlang.Fibers;

	public abstract class AbstractScheduler : IScheduler
	{
		IFiber _fiber;

		public IFiber Fiber
		{
			get { return _fiber ?? (_fiber = BuildFiber()); }
		}

		protected abstract IFiber BuildFiber();

		public void Dispose()
		{
			if (_fiber != null)
				_fiber.Dispose();
		}
	}
}