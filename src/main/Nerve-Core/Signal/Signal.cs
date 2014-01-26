namespace Kostassoid.Nerve.Core.Signal
{
	using System;

	public class Signal<T> : ISignal<T> where T : class
	{
		public T Body { get; private set; }
		object ISignal.Body { get { return Body; } }
		public StackTrace StackTrace { get; private set; }

		public Signal(T body, StackTrace stackTrace)
		{
			Body = body;
			StackTrace = stackTrace;
		}

		public void Return<TResponse>(TResponse response) where TResponse : class
		{
			var sender = StackTrace.Root;
			if (sender == null)
				throw new InvalidOperationException("No agents in stacktrace.");

			sender.Dispatch(response);
		}

		public void Trace(IAgent agent)
		{
			StackTrace.Push(agent);
		}

		public ISignal<TOut> As<TOut>() where TOut : class
		{
			var body = Body as TOut;
			if (body == null)
				throw new InvalidCastException(string.Format("Unable to cast from [{0}] to [{1}].", typeof (T).Name,
					typeof (TOut).Name));

			return new Signal<TOut>(body, StackTrace);
		}
	}
}