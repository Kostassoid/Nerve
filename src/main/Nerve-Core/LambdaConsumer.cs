namespace Kostassoid.Nerve.Core
{
    using System;

    public class LambdaConsumer<T> : IConsumerOf<T>
    {
        private readonly Action<Signal<T>> _handler;

        public LambdaConsumer(Action<Signal<T>> handler)
        {
            _handler = handler;
        }

        public void Handle(Signal<T> context)
        {
            _handler(context);
        }
    }
}