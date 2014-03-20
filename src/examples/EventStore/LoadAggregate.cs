namespace EventStore
{
	using System;

	public class LoadAggregate
	{
		public Type Type { get; private set; } 
		public Guid Id { get; private set; }

		public LoadAggregate(Type type, Guid id)
		{
			Type = type;
			Id = id;
		}
	}
}