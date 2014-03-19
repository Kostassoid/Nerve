namespace EventStore
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking.Operators;
	using Kostassoid.Nerve.Core.Scheduling;

	public class FileBasedEventStore : Cell
	{
		readonly DirectoryInfo _folder = Directory.CreateDirectory("Store");

		public FileBasedEventStore() : base("EventStore", ThreadScheduler.Factory)
		{
			OnStream().Of<UncommitedEventStream>().ReactWith(ProcessUncommited);
		}

		IEnumerable<IDomainEvent> LoadEventStreamFor(Type type, Guid id)
		{
			var path = GetFilePathFor(type.Name, id);
			if (!File.Exists(path))
			{
				return new List<IDomainEvent>();
			}

			return File.ReadLines(path, Encoding.UTF8).Select(
				s =>
					{
						var p = s.Split('~');
						return (IDomainEvent)SimpleJson.DeserializeObject(p[1], Type.GetType(p[0]));
					});
		}

		void SaveEventStream(Type type, Guid id, IEnumerable<IDomainEvent> events)
		{
			var path = GetFilePathFor(type.Name, id);

			var serializedEvents = events.Select(ev => string.Format("{0}~{1}", ev.GetType().FullName, SimpleJson.SerializeObject(ev)));

			File.WriteAllLines(path, serializedEvents, Encoding.UTF8);
		}

		void ProcessUncommited(ISignal<UncommitedEventStream> uncommited)
		{
			var root = uncommited.Payload.Root;
			var loaded = LoadEventStreamFor(root.GetType(), root.Id).ToList();
			var sorted = uncommited.Payload.UncommitedEvents.OrderBy(e => e.Version);

			long currentVersion = loaded.Any() ? loaded.Last().Version : 0;
			foreach (var ev in sorted)
			{
				if (currentVersion != ev.Version)
				{
					break;
				}

				loaded.Add(ev);
				currentVersion++;
			}

			if (loaded.Any())
			{
				SaveEventStream(root.GetType(), root.Id, loaded);
			}
		}

		public T Load<T>(Guid id) where T : AggregateRoot
		{
			var loaded = LoadEventStreamFor(typeof(T), id).ToList();
			if (!loaded.Any())
			{
				throw new InvalidOperationException(string.Format("Aggregate root of type {0} with id {1} not found.", typeof(T).Name, id));
			}

			var root = Activator.CreateInstance<T>();
			foreach (var ev in loaded)
			{
				root.Apply(ev, true);
			}

			return root;
		}

		private string GetFilePathFor(string type, Guid id)
		{
			return Path.Combine(_folder.FullName, string.Format("{0}-{1}.json", type, id));
		}
	}
}