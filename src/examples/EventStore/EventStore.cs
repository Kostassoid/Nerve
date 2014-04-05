namespace EventStore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Processing.Operators;
	using Kostassoid.Nerve.Core.Scheduling;
	using Kostassoid.Nerve.Core.Tpl;
	using Model;

	using Storage;

	public class EventStore : Cell
	{
		readonly IEventStorage _storage;

		public EventStore(IEventStorage storage) : base("EventStore", ThreadScheduler.Factory)
		{
			_storage = storage;

			OnStream().Of<UncommitedEventStream>().ReactWith(ProcessUncommited);
			OnStream().Of<AggregateIdentity>().ReactWith(Load);
		}

		public T Load<T>(Guid id) where T : AggregateRoot
		{
			var taskHandler = new TaskResultHandlerOf<T>();
			Send(new AggregateIdentity(typeof(T), id), taskHandler);
			return taskHandler.TypedTask.Result;
		}

		void Load(ISignal<AggregateIdentity> signal)
		{
			signal.Return(InternalLoad(signal.Payload.Type, signal.Payload.Id));
		}

		void ProcessUncommited(ISignal<UncommitedEventStream> uncommited)
		{
			var root = uncommited.Payload.Root;
			var loaded = _storage.Load(root.GetType(), root.Id).ToList();
			var sorted = uncommited.Payload.UncommitedEvents.OrderBy(e => e.Version);
			var commited = new List<IDomainEvent>();

			var currentVersion = loaded.Any() ? loaded.Last().Version + 1 : 0;
			foreach (var ev in sorted)
			{
				if (currentVersion != ev.Version)
				{
					throw new InvalidOperationException(
						string.Format("Concurrency exception. Expected version of {0} ({1}) to be {2} but got {3}."
						, ev.Type, ev.Id, currentVersion, ev.Version));
				}

				loaded.Add(ev);
				commited.Add(ev);
				currentVersion++;
			}

			if (loaded.Any())
			{
				_storage.Save(root.GetType(), root.Id, loaded);
			}

			uncommited.Return(uncommited.Payload.UncommitedEvents);
		}

		private AggregateRoot InternalLoad(Type type, Guid id)
		{
			var loaded = _storage.Load(type, id).ToList();
			if (!loaded.Any())
			{
				throw new InvalidOperationException(string.Format("Aggregate root of type {0} with id {1} not found.", type.Name, id));
			}

			var root = (AggregateRoot)ConstructInstanceOf(type);
			foreach (var ev in loaded)
			{
				root.Apply(ev, true);
			}

			return root;
		}

		private static object ConstructInstanceOf(Type type)
		{
			var ctor = type
				.GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null, new Type[0], null);

			return ctor.Invoke(null);
		}
	}
}