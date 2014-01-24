namespace Kostassoid.Nerve.Core
{
    using System;

    public interface IProducer
    {
        void Dispatch<T>(T signal);

        void Dispatch<T>(T signal, StackTrace stackTrace);

        ISubscriptionSyntax<object> OnStream();
    }
}