namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Core.Fibers;
	using Core.Fibers.Core;

	[TestFixture]
    public class PoolFiberTests
    {
        [Test]
        public void InOrderExecution()
        {
            PoolFiber fiber = new PoolFiber(new DefaultThreadPool(), new DefaultExecutor());
            fiber.Start();

            int count = 0;
            AutoResetEvent reset = new AutoResetEvent(false);
            List<int> result = new List<int>();
            Action command = delegate
                                  {
                                      result.Add(count++);
                                      if (count == 100)
                                      {
                                          reset.Set();
                                      }
                                  };
            for (int i = 0; i < 100; i++)
            {
                fiber.Enqueue(command);
            }

            Assert.IsTrue(reset.WaitOne(10000, false));
            Assert.AreEqual(100, count);
        }

        [Test]
        public void ExecuteOnlyAfterStart()
        {
            PoolFiber fiber = new PoolFiber();
            AutoResetEvent reset = new AutoResetEvent(false);
            fiber.Enqueue(delegate { reset.Set(); });
            Assert.IsFalse(reset.WaitOne(1, false));
            fiber.Start();
            Assert.IsTrue(reset.WaitOne(1000, false));
            fiber.Stop();
        }
    }
}