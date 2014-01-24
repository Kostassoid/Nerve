namespace Kostassoid.Nerve.Core.Tools
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class ImmutableStack<T> : IStack<T>
    {
        private sealed class EmptyStack : IStack<T>
        {
            public bool IsEmpty { get { return true; } }
            public T Peek() { throw new Exception("Empty stack"); }
            public IStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
            public IStack<T> Pop() { throw new Exception("Empty stack"); }
            public IEnumerator<T> GetEnumerator() { yield break; }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
        private static readonly EmptyStack empty = new EmptyStack();
        public static IStack<T> Empty { get { return empty; } }
        private readonly T _head;
        private readonly IStack<T> _tail;
        private ImmutableStack(T head, IStack<T> tail)
        {
            _head = head;
            _tail = tail;
        }
        public bool IsEmpty { get { return false; } }
        public T Peek() { return _head; }
        public IStack<T> Pop() { return _tail; }
        public IStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
        public IEnumerator<T> GetEnumerator()
        {
            for (IStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
                yield return stack.Peek();
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}