namespace Kostassoid.Nerve.Core
{
	using Pipeline;
	using Signal;

	public interface IProducer
	{
		void Dispatch<T>(T body) where T : class;

		void Dispatch(ISignal signal);

		IPipelineStep OnStream();

		void Subscribe(Link link);
		void Unsubscribe(Link link);
	}
}