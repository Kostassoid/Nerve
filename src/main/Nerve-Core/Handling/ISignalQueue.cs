namespace Kostassoid.Nerve.Core.Handling
{
    using Kostassoid.Nerve.Core.Signal;

    public interface ISignalQueue
    {
        long Length { get; }
        ISignal Next();
    }
}