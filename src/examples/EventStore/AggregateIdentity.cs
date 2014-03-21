namespace EventStore
{
	using System;

	public class AggregateIdentity
	{
		public Type Type { get; private set; } 
		public Guid Id { get; private set; }

		public AggregateIdentity(Type type, Guid id)
		{
			Type = type;
			Id = id;
		}
	}
}