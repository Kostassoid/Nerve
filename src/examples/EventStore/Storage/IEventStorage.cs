namespace EventStore.Storage
{
	using System;
	using System.Collections.Generic;

	using Model;

	public interface IEventStorage
	{
		IEnumerable<IDomainEvent> Load(Type type, Guid id);
		void Save(Type type, Guid id, IEnumerable<IDomainEvent> events);
	}
}