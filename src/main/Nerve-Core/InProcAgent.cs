namespace Kostassoid.Nerve.Core
{
    using System.Reactive.Subjects;
    using Tools;

    public class InProcAgent : IAgent
    {
        private readonly Subject<Signal<object>> _subject = new Subject<Signal<object>>();

        public void Dispose()
        {
        }

        public void Dispatch<T>(T signal)
        {
            var s = new Signal<object>(signal, new StackTrace(this));
            _subject.OnNext(s);
        }

        public void Dispatch<T>(T signal, StackTrace stackTrace)
        {
            var s = new Signal<object>(signal, stackTrace);
            _subject.OnNext(s);
        }

        public ISubscriptionSyntax<object> OnStream()
        {
            return new Subscription<object>();
        }

        public void Handle(Signal<object> context)
        {
            Dispatch(new Signal<object>(context.Body, context.StackTrace.Push(this)));
        }
    }
}