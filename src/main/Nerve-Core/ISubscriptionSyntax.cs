namespace Kostassoid.Nerve.Core
{
    using System;

    public interface ISubscriptionSyntax<T>
    {
        ISubscriptionSyntax<TTarget> Of<TTarget>();
        //ISubscriptionSyntax<TTarget> Where<TTarget>();
        Action ReactWith(IConsumerOf<T> consumer);
        Action ReactWith(Action<Signal<T>> handler);
        Action ReactWith(IAgent agent);
    }

    internal class Subscription<T> : ISubscriptionSyntax<T>
    {
        public ISubscriptionSyntax<TTarget> Of<TTarget>()
        {
            throw new NotImplementedException();
        }

        public Action ReactWith(IConsumerOf<T> consumer)
        {
            throw new NotImplementedException();
        }

        public Action ReactWith(Action<Signal<T>> handler)
        {
            return ReactWith(new LambdaConsumer<T>(handler));
        }
        public Action ReactWith(IAgent agent)
        {
            throw new NotImplementedException();
        }

    }
}