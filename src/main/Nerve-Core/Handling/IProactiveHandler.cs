namespace Kostassoid.Nerve.Core.Handling
{
    public interface IProactiveHandler
    {
        void Handle(ISignalQueue queue);
    }
}