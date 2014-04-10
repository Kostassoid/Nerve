namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using System;
	using System.Threading;

	using Core.Fibers.Core;

	using Machine.Specifications;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class BoundedQueueSpecs
    {

		[Subject(typeof(BoundedQueue))]
		[Tags("Unit")]
	    public class when_action_on_bounded_queue_throws_exception
		{
			It should_throw = () =>
				{
					var exception = new Exception();
					Action failingAction = () => { throw exception; };

					var queue = new DefaultQueue();
					queue.Enqueue(failingAction);
					var caughtException = Catch.Exception(() => queue.ExecuteNextBatch());
					caughtException.ShouldEqual(exception);
				};
		}

	    [Subject(typeof(BoundedQueue))]
	    [Tags("Unit")]
	    public class when_bounded_queue_is_stopped
	    {
		    It should_not_execute_actions = () =>
			    {
				    var counter = 0;

				    Action incrementingAction = () => counter++;

					var queue = new DefaultQueue();
					queue.Enqueue(incrementingAction);

				    counter.ShouldEqual(0);

				    var run = new Thread(queue.Run);
				    run.Start();
				    Thread.Sleep(100);

					counter.ShouldEqual(1);

				    queue.Enqueue(incrementingAction);
					Thread.Sleep(100);

					queue.Stop();

					counter.ShouldEqual(2);

				    queue.Enqueue(incrementingAction);

				    Thread.Sleep(100);
				    run.Join();

					counter.ShouldEqual(2);
			    };
	    }

	    [Subject(typeof(BoundedQueue))]
	    [Tags("Unit")]
	    public class when_hitting_the_depth_limit_of_bounded_queue
	    {
		    It should_throw = () =>
			    {
				    var queue = new BoundedQueue { MaxDepth = 2 };

				    queue.Enqueue(delegate { });
				    queue.Enqueue(delegate { });

				    var exception = Catch.Exception(() => queue.Enqueue(delegate { }));

					exception.ShouldBeOfExactType<QueueFullException>();
			    };
	    }
    }
	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}