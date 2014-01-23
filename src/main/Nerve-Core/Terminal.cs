namespace Kostassoid.Nerve.Core
{
    using System;

    public abstract class Terminal<T> : IConsumerOf<T>
    {
        public abstract void OnNext(IConsumingContextOf<T> value);
        public abstract void OnError(Exception error);
        public abstract void OnCompleted();
    }
}