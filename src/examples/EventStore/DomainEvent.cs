namespace EventStore
{
	using System;

	public abstract class DomainEvent : IDomainEvent
	{
		public DateTime Happened { get; set; }
		public string Type { get; set; }
		public Guid Id { get; set; }
		public long Version { get; set; }

		protected DomainEvent()
		{}

		protected DomainEvent(string type, Guid id, long version)
		{
			Type = type;
			Id = id;
			Version = version;

			Happened = DateTime.UtcNow;
		}

		protected DomainEvent(AggregateRoot root) : this(root.GetType().Name, root.Id, root.Version)
		{
		}
	}

}