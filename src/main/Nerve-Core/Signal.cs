namespace Kostassoid.Nerve.Core
{
    using System;
    using Tools;

    public class Signal<T>
    {
        public T Body { get; private set; }
        public StackTrace StackTrace { get; private set; }

        public Signal(T body, StackTrace stackTrace)
        {
            Body = body;
            StackTrace = stackTrace;
        }

        public void Return<TResponse>(TResponse response)
        {
            var sender = StackTrace.Head;
            if (sender == null)
                throw new InvalidOperationException("No agents in stacktrace.");

            sender.Dispatch(response, StackTrace.Pop());
        }
    }
}