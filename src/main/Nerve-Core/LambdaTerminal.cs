namespace Kostassoid.Nerve.Core
{
    using System;

    public class LambdaTerminal<T> : Terminal<T>
    {
        private readonly Action<IConsumingContextOf<T>> _onNext;

        public LambdaTerminal(Action<IConsumingContextOf<T>> onNext)
        {
            _onNext = onNext;
        }

        public override void OnNext(IConsumingContextOf<T> value)
        {
            _onNext(value);
        }

        public override void OnError(Exception error)
        {
        }

        public override void OnCompleted()
        {
        }
    }
}