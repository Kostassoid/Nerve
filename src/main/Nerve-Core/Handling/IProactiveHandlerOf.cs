namespace Kostassoid.Nerve.Core.Handling
{
    using Linking;

    public interface IProactiveHandlerOf<in T> where T : class
    {
        void Handle(ISignalQueueOf<T> queue);
    }
}