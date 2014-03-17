namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using Core.Fibers;

	[TestFixture]
    public class ThreadFiberTests
    {
        [Test]
        public void RunThread()
        {
            ThreadFiber threadFiber = new ThreadFiber();
            threadFiber.Start();
            threadFiber.Dispose();
            threadFiber.Join();
        }

        [Test]
        public void AsyncStop()
        {
            ThreadFiber threadFiber = new ThreadFiber();
            threadFiber.Start();
            threadFiber.Enqueue(threadFiber.Dispose);
            threadFiber.Join();
        }
    }
}