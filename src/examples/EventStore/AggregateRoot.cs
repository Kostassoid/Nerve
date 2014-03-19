namespace EventStore
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public abstract class AggregateRoot
	{
		private IList<IDomainEvent> _uncommited = new List<IDomainEvent>();

		public Guid Id { get; protected set; }
		public long Version { get; protected set; }

		protected AggregateRoot()
		{
			Version = 0;
		}

		protected AggregateRoot(Guid id)
		{
			Id = id;
		}

		public void Apply(IDomainEvent ev, bool isReplaying = false)
		{
			GetType().InvokeMember(
				"On" + ev.GetType().Name,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
				null, this, new object[] { ev });

			Version++;

			if (!isReplaying)
			{
				_uncommited.Add(ev);
			}
		}

		public void Commit()
		{
			// possible race condition here?
			var toCommit = _uncommited;
			_uncommited = new List<IDomainEvent>();

			EventBus.Raise(new UncommitedEventStream(this, toCommit));
		}
	}
}