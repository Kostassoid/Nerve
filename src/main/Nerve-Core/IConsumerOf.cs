namespace Kostassoid.Nerve.Core
{
	using Signal;

	public interface IConsumerOf<in T> : IConsumer where T : class
	{
		void Handle(ISignal<T> signal);
	}
}