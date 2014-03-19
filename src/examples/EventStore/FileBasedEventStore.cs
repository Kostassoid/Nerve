namespace EventStore
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking.Operators;
	using Kostassoid.Nerve.Core.Scheduling;
	using Newtonsoft.Json;

	public class FileBasedEventStore : Cell
	{
		readonly DirectoryInfo _folder = Directory.CreateDirectory("Store");

		public FileBasedEventStore() : base("EventStore", ThreadScheduler.Factory)
		{
			ClearStorageFolder();

			OnStream().Of<UncommitedEventStream>().ReactWith(ProcessUncommited);
		}

		void ClearStorageFolder()
		{
			_folder.Delete(true);
			_folder.Create();
		}

		IEnumerable<IDomainEvent> LoadEventStreamFor(Type type, Guid id)
		{
			var path = GetFilePathFor(type.Name, id);
			if (!File.Exists(path))
			{
				return new List<IDomainEvent>();
			}

			var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto};
			return (IEnumerable<IDomainEvent>)JsonConvert.DeserializeObject(File.ReadAllText(path, Encoding.UTF8), typeof(IEnumerable<IDomainEvent>), settings);
		}

		void SaveEventStream(Type type, Guid id, IEnumerable<IDomainEvent> events)
		{
			var path = GetFilePathFor(type.Name, id);

			var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented};
			File.WriteAllText(path, JsonConvert.SerializeObject(events, settings));
		}

		void ProcessUncommited(ISignal<UncommitedEventStream> uncommited)
		{
			var root = uncommited.Payload.Root;
			var loaded = LoadEventStreamFor(root.GetType(), root.Id).ToList();
			var sorted = uncommited.Payload.UncommitedEvents.OrderBy(e => e.Version);

			long currentVersion = loaded.Any() ? loaded.Last().Version + 1 : 0;
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

			var root = ConstructInstanceOf<T>();
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

		private static T ConstructInstanceOf<T>()
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var ctor = typeof(T).GetConstructor(flags, null, new Type[0], null);
			return (T)ctor.Invoke(null);
		}
	}
}