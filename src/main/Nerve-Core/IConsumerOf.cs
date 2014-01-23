namespace Kostassoid.Nerve.Core
{
    using System;

    public interface IConsumerOf<in T> : IObserver<IConsumingContextOf<T>>
    {
    }
}