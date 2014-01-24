namespace Kostassoid.Nerve.Core
{
    public interface IConsumerOf<T>
    {
        void Handle(Signal<T> context);
    }
}