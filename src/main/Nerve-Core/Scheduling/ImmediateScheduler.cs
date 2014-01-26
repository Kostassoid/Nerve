namespace Kostassoid.Nerve.Core.Scheduling
{
	using Retlang.Fibers;

	public class ImmediateScheduler : AbstractScheduler
	{
		protected override IFiber BuildFiber()
		{
			return new StubFiber {ExecutePendingImmediately = true};
		}
	}
}