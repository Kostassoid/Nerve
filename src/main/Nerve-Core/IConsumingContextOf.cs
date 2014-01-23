namespace Kostassoid.Nerve.Core
{
    public interface IConsumingContextOf<out T>
    {
        T Message { get; }

        void Reply<TResponse>(TResponse message);
    }
}