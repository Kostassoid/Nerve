namespace Kostassoid.Nerve.Core.Specs
{
	using System.Diagnostics;
	using System.Threading;
	using Machine.Specifications;
	using Model;
	using Pipeline;
	using Scheduling;

	// ReSharper disable InconsistentNaming
	// ReSharper disable UnusedMember.Local
	public class PerformanceSpecs
	{
		[Subject(typeof(IAgent), "Performance")]
		[Tags("Unit")]
		public class when_publishing_many_signals_on_one_agent_using_immediate_scheduler
		{
			Establish context = () =>
			{
				_agent = new Agent();
				_agent.OnStream().Of<Ping>().Through(new ImmediateScheduler()).ReactWith(_ => _countdown.Signal());
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () =>
			{
				_stopwatch.Start();
				for (var i = 0; i < _signals; i++)
					_agent.Dispatch(new Ping());

				_countdown.Wait();
				_stopwatch.Stop();
			};

			It should_be_faster_than_1_million_ops = () => (_signals / _stopwatch.ElapsedMilliseconds * 1000).ShouldBeGreaterThan(1000000);

			const int _signals = 1000000;
			static readonly CountdownEvent _countdown = new CountdownEvent(_signals);
			static readonly Stopwatch _stopwatch = new Stopwatch();
			static IAgent _agent;
		}

		[Subject(typeof(IAgent), "Performance")]
		[Tags("Unit")]
		public class when_publishing_many_signals_on_one_agent_using_pool_scheduler
		{
			Establish context = () =>
			{
				_agent = new Agent();
				_agent.OnStream().Of<Ping>().Through(new PoolScheduler()).ReactWith(_ => _countdown.Signal());
			};

			Cleanup after = () => _agent.Dispose();

			Because of = () =>
			{
				_stopwatch.Start();
				for (var i = 0; i < _signals; i++)
					_agent.Dispatch(new Ping());

				_countdown.Wait();
				_stopwatch.Stop();
			};

			It should_be_faster_than_1_million_ops = () => (_signals / _stopwatch.ElapsedMilliseconds * 1000).ShouldBeGreaterThan(1000000);

			const int _signals = 1000000;
			static readonly CountdownEvent _countdown = new CountdownEvent(_signals);
			static readonly Stopwatch _stopwatch = new Stopwatch();
			static IAgent _agent;
		}

	}

	// ReSharper restore InconsistentNaming
	// ReSharper restore UnusedMember.Local
}