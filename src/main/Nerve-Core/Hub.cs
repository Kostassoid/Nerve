namespace Kostassoid.Nerve.Core
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class Hub : IDisposable
    {
        private readonly Subject<object> _subject = new Subject<object>();

        public void Dispose()
        {
        }

        public void Dispatch<T>(T message, Expect expectations = null)
        {
            _subject.OnNext(new ConsumingContextOf<T>(message, expectations));
        }

        public IObservable<IConsumingContextOf<T>> On<T>()
        {
            return _subject
                .Where(o => o is IConsumingContextOf<T>)
                .Select(o => (IConsumingContextOf<T>)o)
                .AsObservable();
        }
    }
}