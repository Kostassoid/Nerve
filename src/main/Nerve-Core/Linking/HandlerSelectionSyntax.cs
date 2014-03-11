namespace Kostassoid.Nerve.Core.Linking
{
    using System;
    using Handling;
    using Operators;
    using Signal;

    class HandlerSelectionSyntax : IHandlerSelectionSyntax
    {
        private readonly ILinkContinuation _step;

        public HandlerSelectionSyntax(ILinkContinuation step)
        {
            _step = step;
        }

        public IDisposable Reactively(IReactiveHandler handler)
        {
            var next = new HandleOperator(handler);
            _step.Attach(next);

            //TODO: not pretty
            return _step.Link.AttachToCell();
        }

        public IDisposable Proactively(IProactiveHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    class HandlerSelectionSyntax<T> : IHandlerSelectionSyntax<T> where T : class
    {
        private readonly ILinkContinuation _step;

        public HandlerSelectionSyntax(ILinkContinuation step)
        {
            _step = step;
        }

        public IDisposable Reactively(IReactiveHandlerOf<T> handler)
        {
            var next = new HandleOperator<T>(handler);
            _step.Attach(next);

            //TODO: not pretty
            return _step.Link.AttachToCell();
        }

        public IDisposable Reactively(Action<ISignal<T>> handler, Action<SignalHandlingException> failureHandler = null)
        {
            return Reactively(new LambdaReactiveHandler<T>(handler, failureHandler));
        }


        public IDisposable Proactively(IProactiveHandlerOf<T> queue)
        {
            throw new NotImplementedException();
        }

    }
}