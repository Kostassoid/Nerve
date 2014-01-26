namespace Kostassoid.Nerve.Core.Scheduling
{
	using Retlang.Fibers;

	public class PoolScheduler : AbstractScheduler
	{
		protected override IFiber BuildFiber()
		{
			return new PoolFiber();
		}
	}
}