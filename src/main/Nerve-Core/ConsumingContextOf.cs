namespace Kostassoid.Nerve.Core
{
    public class ConsumingContextOf<T> : IConsumingContextOf<T>
    {
        public T Message { get; private set; }

        public Expect Expectations { get; private set; }

        public ConsumingContextOf(T message, Expect expectations)
        {
            Message = message;
            Expectations = expectations;
        }

        public void Reply<TResponse>(TResponse message)
        {
            Expectations.Dispatch(message);
        }

    }
}