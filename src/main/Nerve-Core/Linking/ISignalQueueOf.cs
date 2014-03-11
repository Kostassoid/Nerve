namespace Kostassoid.Nerve.Core.Linking
{
    using Signal;

    public interface ISignalQueueOf<out T> where T : class
    {
        long Length { get; }
        ISignal<T> Next();
    }
}