namespace EventStore
{
	using System;

	public abstract class DomainEvent : IDomainEvent
	{
		public string Type { get; private set; }
		public Guid Id { get; private set; }
		public long Version { get; private set; }

		protected DomainEvent()
		{}

		protected DomainEvent(string type, Guid id, long version)
		{
			Type = type;
			Id = id;
			Version = version;
		}

		protected DomainEvent(AggregateRoot root) : this(root.GetType().Name, root.Id, root.Version)
		{
		}
	}

}