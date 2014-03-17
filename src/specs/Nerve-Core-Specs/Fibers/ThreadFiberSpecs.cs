namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using Core.Fibers;

	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class ThreadFiberSpecs
    {
		[Subject(typeof(ThreadFiber))]
		[Tags("Unit")]
		public class when_running_thread_fiber
		{
			It should_run_to_completion = () =>
				{
					ThreadFiber threadFiber = new ThreadFiber();
					threadFiber.Start();
					threadFiber.Dispose();
					threadFiber.Join();
				};
		}

		[Subject(typeof(ThreadFiber))]
		[Tags("Unit")]
		public class when_disposing_thread_fiber_async
		{
			It should_run_to_completion = () =>
				{
					ThreadFiber threadFiber = new ThreadFiber();
					threadFiber.Start();
					threadFiber.Enqueue(threadFiber.Dispose);
					threadFiber.Join();
				};
		}
    }
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}