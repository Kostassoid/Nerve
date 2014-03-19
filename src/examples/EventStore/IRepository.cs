namespace EventStore
{
	using System;

	public interface IRepository<T> where T : AggregateRoot
	{
		void Save(T root);
		T Get(Guid id);
	}
}