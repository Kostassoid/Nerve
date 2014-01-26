namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;

	public class StackTrace
	{
		readonly IList<IAgent> _stack = new List<IAgent>();

		public StackTrace(IAgent root)
		{
			_stack.Add(root);
		}

		protected StackTrace(IList<IAgent> stack)
		{
			_stack = stack;
		}

		public IAgent Root
		{
			get { return _stack[0]; }
		}

		public void Push(IAgent agent)
		{
			_stack.Add(agent);
		}
	}
}