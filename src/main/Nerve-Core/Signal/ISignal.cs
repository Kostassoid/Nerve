namespace Kostassoid.Nerve.Core.Signal
{
	public interface ISignal
	{
		object Body { get; }
		StackTrace StackTrace { get; }

		ISignal<T> As<T>() where T : class;

		void Return<TResponse>(TResponse body) where TResponse : class;
		void Trace(IAgent agent);
	}

	public interface ISignal<out T> : ISignal where T : class
	{
		new T Body { get; }
	}

}