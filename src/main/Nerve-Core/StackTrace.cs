namespace Kostassoid.Nerve.Core
{
    using Tools;

    public class StackTrace
    {
        readonly IStack<IAgent> _stack;

        public StackTrace(IAgent root)
        {
            _stack = ImmutableStack<IAgent>.Empty.Push(root);
        }

        protected StackTrace(IStack<IAgent> stack)
        {
            _stack = stack;
        }

        public IAgent Head { get { return _stack.Peek(); } }

        public StackTrace Pop()
        {
            return new StackTrace(_stack.Pop());
        }

        public StackTrace Push(IAgent agent)
        {
            return new StackTrace(_stack.Push(agent));
        }
    }
}