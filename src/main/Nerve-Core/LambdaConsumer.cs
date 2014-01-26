namespace Kostassoid.Nerve.Core
{
	using System;
	using Signal;

	public class LambdaConsumer<T> : IConsumerOf<T> where T : class
	{
		readonly Action<ISignal<T>> _handler;

		public LambdaConsumer(Action<ISignal<T>> handler)
		{
			_handler = handler;
		}

		public void Handle(ISignal<T> signal)
		{
			_handler(signal);
		}

		public void Handle(ISignal signal)
		{
			_handler(signal.As<T>());
		}
	}
}