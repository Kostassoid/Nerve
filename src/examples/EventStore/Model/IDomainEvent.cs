namespace EventStore.Model
{
	using System;

	public interface IDomainEvent
	{
		string Type { get; }
		Guid Id { get; }
		long Version { get; }
	}
}