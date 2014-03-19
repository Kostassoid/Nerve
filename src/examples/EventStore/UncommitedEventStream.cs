namespace EventStore
{
	using System.Collections.Generic;

	public class UncommitedEventStream
	{
		public AggregateRoot Root { get; private set; }
		public IList<IDomainEvent> UncommitedEvents { get; private set; }

		public UncommitedEventStream(AggregateRoot root, IList<IDomainEvent> uncommitedEvents)
		{
			Root = root;
			UncommitedEvents = uncommitedEvents;
		}
	}
}