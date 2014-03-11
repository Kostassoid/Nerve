namespace Kostassoid.Nerve.Core.Linking
{
    using System;
    using Handling;

    public interface IHandlerSelectionSyntax<out T> where T : class
    {
        IDisposable Reactively(IReactiveHandlerOf<T> handler);
        IDisposable Proactively(IProactiveHandlerOf<T> queue);
    }

    public interface IHandlerSelectionSyntax
    {
        IDisposable Reactively(IReactiveHandler handler);
        IDisposable Proactively(IProactiveHandler handler);
    }
}