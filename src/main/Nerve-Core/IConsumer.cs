namespace Kostassoid.Nerve.Core
{
	using Signal;

	public interface IConsumer
	{
		void Handle(ISignal signal);
	}
}