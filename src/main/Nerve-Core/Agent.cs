namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;
	using Pipeline;
	using Signal;
	using Tools;

	public class Agent : IAgent
	{
		public string Name { get; private set; }
		readonly ISet<Link> _links = new HashSet<Link>();

		public Agent(string name = null)
		{
			Name = name;
		}

		public void Dispose()
		{
			_links.ForEach(l => l.Dispose());
			_links.Clear();
		}

		public void Dispatch<T>(T body) where T : class
		{
			Dispatch(new Signal<T>(body, new StackTrace(this)) as ISignal);
		}

		public void Dispatch(ISignal signal)
		{
			_links.ForEach(l => l.Process(signal));
		}

		public IPipelineStep OnStream()
		{
			return new Link(this).PipelineStep;
		}

		public void Subscribe(Link link)
		{
			_links.Add(link);
		}

		public void Unsubscribe(Link link)
		{
			_links.Remove(link);
			link.Dispose();
		}

		public void Handle(ISignal signal)
		{
			signal.Trace(this);
			Dispatch(signal);
		}

		public override string ToString()
		{
			return string.Format("Agent [{0}]", Name ?? "unnamed");
		}
	}
}