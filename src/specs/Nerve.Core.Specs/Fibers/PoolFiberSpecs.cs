namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Core.Fibers;
	using Core.Fibers.Core;

	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class PoolFiberSpecs
    {
		[Subject(typeof(PoolFiber))]
		[Tags("Unit")]
		public class when_enqueuing_on_pool_fiber
		{
			It should_execute_in_original_order = () =>
				{
					var fiber = new PoolFiber(new DefaultThreadPool(), new DefaultExecutor());
					fiber.Start();

					var count = 0;
					var reset = new AutoResetEvent(false);
					var result = new List<int>();
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

					reset.WaitOne(10000, false).ShouldBeTrue();
					count.ShouldEqual(100);
				};

			It should_execute_only_after_start = () =>
			{
				var fiber = new PoolFiber();
				var reset = new AutoResetEvent(false);
				fiber.Enqueue(() => reset.Set());
				reset.WaitOne(1, false).ShouldBeFalse();
				fiber.Start();
				reset.WaitOne(1000, false).ShouldBeTrue();
				fiber.Stop();
			};
		}
    }
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}